using DA_Assets.FCU.Drawers.CanvasDrawers;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using DA_Assets.SVGMeshUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteGenerator : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] Camera camera;

        private float spriteGenerationDelay = 0.25f;
        private int meshUpscaleFactor = 16;
        private int renderAntialiasing = 8;
        private float blurCoof = 10f;

        private FilterMode filterMode = FilterMode.Bilinear;
        private TextureFormat textureFormat = TextureFormat.ARGB32;
        private RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

        public IEnumerator GenerateSprites(List<FObject> fobjects)
        {
            List<FObject> generative = fobjects.Where(x => x.Data.NeedGenerate).ToList();

            if (generative.IsEmpty())
                yield break;

            if (camera == null)
            {
                GameObject cameraGo = MonoBehExtensions.CreateEmptyGameObject();
                cameraGo.name = "SpriteGeneratorCamera";
                cameraGo.TryAddComponent(out camera);
                camera.orthographic = true;
                camera.backgroundColor = new Color(0, 0, 0, 0);
                camera.clearFlags = CameraClearFlags.Color;
            }

            int generatedCount = 0;

            DACycles.ForEach(generative, fobject =>
            {
                monoBeh.Log($"GenerateSprites | {fobject.Data.Hierarchy} | {fobject.Data.NeedGenerate}");

                try
                {
                    Texture2D fillTexture = null;
                    Texture2D strokeTexture = null;
                    Texture2D finalTexture = null;

                    FGraphic graphic = fobject.GetGraphic();

                    Vector2Int finalSize = default;

                    if (graphic.HasFill)
                    {
                        Vector2 fillSize = fobject.Size;
                        fillSize -= new Vector2Int(1, 0);
                        fillSize.IsSupportedRenderSize(monoBeh, out finalSize, out Vector2Int bakeFillSize);

                        string fillPath = fobject.FillGeometry[0].Path;

                        Color textureColor;

                        if (graphic.HasFill && graphic.HasStroke)
                        {
                            textureColor = graphic.SolidFill.Color;
                        }
                        else
                        {
                            textureColor = Color.white;
                        }

                        fillTexture = GenerateTexture(fillPath, fillSize, bakeFillSize, textureColor);
                    }

                    if (graphic.HasStroke && fobject.StrokeAlign != StrokeAlign.OUTSIDE)
                    {
                        Vector2 strokeSize = fobject.Size;
                        strokeSize += new Vector2(fobject.StrokeWeight * 2, fobject.StrokeWeight * 2);
                        strokeSize.IsSupportedRenderSize(monoBeh, out finalSize, out Vector2Int bakeStrokeSize);

                        string strokePath = fobject.StrokeGeometry[0].Path;

                        Color textureColor;

                        if (graphic.HasFill && graphic.HasStroke)
                        {
                            textureColor = graphic.SolidStroke.Color;
                        }
                        else
                        {
                            textureColor = Color.white;
                        }

                        strokeTexture = GenerateTexture(strokePath, strokeSize, bakeStrokeSize, textureColor);
                    }

                    monoBeh.Log($"GenerateSprites | {fobject.Data.Hierarchy} | hasFills: {graphic.HasFill} | hasStrokes: {graphic.HasStroke}", FcuLogType.Default);

                    if (fillTexture != null && strokeTexture != null)
                    {
                        finalTexture = strokeTexture.Merge(fillTexture);
                    }
                    else if (strokeTexture != null)
                    {
                        finalTexture = strokeTexture;
                    }
                    else if (fillTexture != null)
                    {
                        finalTexture = fillTexture;
                    }

                    finalTexture.Blur(monoBeh.Settings.MainSettings.ImageScale / blurCoof);
                    finalTexture.Resize(finalSize, 0, filterMode, renderTextureFormat);

                    byte[] textureBytes = finalTexture.EncodeToPNG();
                    finalTexture.Destroy();

                    File.WriteAllBytes(fobject.Data.SpritePath, textureBytes);
                }
                catch (Exception ex)
                {
                    monoBeh.Log($"Can't generate '{fobject.Data.Hierarchy}'\n{ex}", FcuLogType.Error);
                    fobject.Data.FcuImageType = FcuImageType.Drawable;
                }

                generatedCount++;
            }, spriteGenerationDelay, 1).StartDARoutine(monoBeh);

            while (true)
            {
                DALogger.Log(FcuLocKey.log_generating_sprites.Localize(generatedCount, generative.Count));

                if (generatedCount >= generative.Count)
                    break;

                yield return WaitFor.Delay1();
            }

            camera.gameObject.Destroy();
        }

        public Texture2D GenerateTexture(string svgPath, Vector2 sourceSize, Vector2Int bakeResolution, Color color)
        {
            GameObject meshObject = MonoBehExtensions.CreateEmptyGameObject();
            meshObject.name = sourceSize.ToString();
            meshObject.transform.position = new Vector3(-20000, -20000, 0);

            try
            {
                GenerateMesh(meshObject, sourceSize, svgPath);
                Texture2D bakedTexture = BakeTexture(meshObject, bakeResolution, color);
                return bakedTexture;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                meshObject.Destroy();
            }
        }

        private void GenerateMesh(GameObject meshObject, Vector2 objectSize, string svgPath)
        {
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();

#if UNITY_EDITOR
            Material spriteMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            meshRenderer.material = spriteMaterial;
#endif

            SVGMesh svgMesh = new SVGMesh();
            svgMesh.Init(meshUpscaleFactor);

            SVGData svgData = new SVGData();
            svgData.Path(svgPath);
            svgMesh.Fill(svgData, meshFilter);
        }

        private Texture2D BakeTexture(GameObject meshObject, Vector2Int bakeResolution, Color color)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(bakeResolution.x, bakeResolution.y, 8, renderTextureFormat);

            renderTexture.antiAliasing = renderAntialiasing;
            renderTexture.filterMode = filterMode;

            camera.targetTexture = renderTexture;
            camera.SetToObject(meshObject);
            camera.Render();

            Texture2D texture = new Texture2D(bakeResolution.x, bakeResolution.y, textureFormat, false);
            texture.filterMode = filterMode;

            RenderTexture.active = renderTexture;

            texture.ReadPixels(new Rect(0, 0, bakeResolution.x, bakeResolution.y), 0, 0);
            texture.Apply();

            texture.Colorize(color);

            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.active = null;
            camera.targetTexture = null;

            return texture;
        }
    }

    public static class SpriteGeneratorExtensions
    {
        public static void SetToObject(this Camera camera, GameObject target)
        {
            target.transform.position = new Vector3((int)target.transform.position.x, (int)target.transform.position.y, (int)target.transform.position.z);

            Renderer objectRenderer = target.GetComponent<Renderer>();
            Vector3 objectSize = objectRenderer.bounds.size;
            Vector3 objectPosition = objectRenderer.bounds.center;

            camera.transform.position = new Vector3(objectPosition.x, objectPosition.y, -1);
            camera.orthographicSize = Mathf.Max(objectSize.x / (2f * camera.aspect), objectSize.y / 2f);
        }
    }
}
