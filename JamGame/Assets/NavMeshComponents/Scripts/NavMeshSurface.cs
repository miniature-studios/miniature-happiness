using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UnityEngine.AI
{
    public enum CollectObjects
    {
        All = 0,
        Volume = 1,
        Children = 2,
    }

    [ExecuteAlways]
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Navigation/NavMeshSurface", 30)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshSurface : MonoBehaviour
    {
        [SerializeField]
        private int m_AgentTypeID;
        public int agentTypeID
        {
            get => m_AgentTypeID;
            set => m_AgentTypeID = value;
        }

        [SerializeField]
        private CollectObjects m_CollectObjects = CollectObjects.All;
        public CollectObjects collectObjects
        {
            get => m_CollectObjects;
            set => m_CollectObjects = value;
        }

        [SerializeField]
        private Vector3 m_Size = new(10.0f, 10.0f, 10.0f);
        public Vector3 size
        {
            get => m_Size;
            set => m_Size = value;
        }

        [SerializeField]
        private Vector3 m_Center = new(0, 2.0f, 0);
        public Vector3 center
        {
            get => m_Center;
            set => m_Center = value;
        }

        [SerializeField]
        private LayerMask m_LayerMask = ~0;
        public LayerMask layerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }

        [SerializeField]
        private NavMeshCollectGeometry m_UseGeometry = NavMeshCollectGeometry.RenderMeshes;
        public NavMeshCollectGeometry useGeometry
        {
            get => m_UseGeometry;
            set => m_UseGeometry = value;
        }

        [SerializeField]
        private int m_DefaultArea;
        public int defaultArea
        {
            get => m_DefaultArea;
            set => m_DefaultArea = value;
        }

        [SerializeField]
        private bool m_IgnoreNavMeshAgent = true;
        public bool ignoreNavMeshAgent
        {
            get => m_IgnoreNavMeshAgent;
            set => m_IgnoreNavMeshAgent = value;
        }

        [SerializeField]
        private bool m_IgnoreNavMeshObstacle = true;
        public bool ignoreNavMeshObstacle
        {
            get => m_IgnoreNavMeshObstacle;
            set => m_IgnoreNavMeshObstacle = value;
        }

        [SerializeField]
        private bool m_OverrideTileSize;
        public bool overrideTileSize
        {
            get => m_OverrideTileSize;
            set => m_OverrideTileSize = value;
        }

        [SerializeField]
        private int m_TileSize = 256;
        public int tileSize
        {
            get => m_TileSize;
            set => m_TileSize = value;
        }

        [SerializeField]
        private bool m_OverrideVoxelSize;
        public bool overrideVoxelSize
        {
            get => m_OverrideVoxelSize;
            set => m_OverrideVoxelSize = value;
        }

        [SerializeField]
        private float m_VoxelSize;
        public float voxelSize
        {
            get => m_VoxelSize;
            set => m_VoxelSize = value;
        }

        // Currently not supported advanced options
        [SerializeField]
        private bool m_BuildHeightMesh;
        public bool buildHeightMesh
        {
            get => m_BuildHeightMesh;
            set => m_BuildHeightMesh = value;
        }

        // Reference to whole scene navmesh data asset.
        [UnityEngine.Serialization.FormerlySerializedAs("m_BakedNavMeshData")]
        [SerializeField]
        private NavMeshData m_NavMeshData;
        public NavMeshData navMeshData
        {
            get => m_NavMeshData;
            set => m_NavMeshData = value;
        }

        // Do not serialize - runtime only state.
        private NavMeshDataInstance m_NavMeshDataInstance;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;
        private static readonly List<NavMeshSurface> s_NavMeshSurfaces = new();

        public static List<NavMeshSurface> activeSurfaces => s_NavMeshSurfaces;

        private void OnEnable()
        {
            Register(this);
            AddData();
        }

        private void OnDisable()
        {
            RemoveData();
            Unregister(this);
        }

        public void AddData()
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(this);
            if (isPrefab)
            {
                //Debug.LogFormat("NavMeshData from {0}.{1} will not be added to the NavMesh world because the gameObject is a prefab.",
                //    gameObject.name, name);
                return;
            }
#endif
            if (m_NavMeshDataInstance.valid)
            {
                return;
            }

            if (m_NavMeshData != null)
            {
                m_NavMeshDataInstance = NavMesh.AddNavMeshData(
                    m_NavMeshData,
                    transform.position,
                    transform.rotation
                );
                m_NavMeshDataInstance.owner = this;
            }

            m_LastPosition = transform.position;
            m_LastRotation = transform.rotation;
        }

        public void RemoveData()
        {
            m_NavMeshDataInstance.Remove();
            m_NavMeshDataInstance = new NavMeshDataInstance();
        }

        public NavMeshBuildSettings GetBuildSettings()
        {
            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning("No build settings for agent type ID " + agentTypeID, this);
                buildSettings.agentTypeID = m_AgentTypeID;
            }

            if (overrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = tileSize;
            }
            if (overrideVoxelSize)
            {
                buildSettings.overrideVoxelSize = true;
                buildSettings.voxelSize = voxelSize;
            }
            return buildSettings;
        }

        public void BuildNavMesh()
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            Bounds sourcesBounds = new(m_Center, Abs(m_Size));
            if (m_CollectObjects is CollectObjects.All or CollectObjects.Children)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            NavMeshData data = NavMeshBuilder.BuildNavMeshData(
                GetBuildSettings(),
                sources,
                sourcesBounds,
                transform.position,
                transform.rotation
            );

            if (data != null)
            {
                data.name = gameObject.name;
                RemoveData();
                m_NavMeshData = data;
                if (isActiveAndEnabled)
                {
                    AddData();
                }
            }
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            Bounds sourcesBounds = new(m_Center, Abs(m_Size));
            if (m_CollectObjects is CollectObjects.All or CollectObjects.Children)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            return NavMeshBuilder.UpdateNavMeshDataAsync(
                data,
                GetBuildSettings(),
                sources,
                sourcesBounds
            );
        }

        private static void Register(NavMeshSurface surface)
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(surface);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(surface);
            if (isPrefab)
            {
                //Debug.LogFormat("NavMeshData from {0}.{1} will not be added to the NavMesh world because the gameObject is a prefab.",
                //    surface.gameObject.name, surface.name);
                return;
            }
#endif
            if (s_NavMeshSurfaces.Count == 0)
            {
                NavMesh.onPreUpdate += UpdateActive;
            }

            if (!s_NavMeshSurfaces.Contains(surface))
            {
                s_NavMeshSurfaces.Add(surface);
            }
        }

        private static void Unregister(NavMeshSurface surface)
        {
            _ = s_NavMeshSurfaces.Remove(surface);

            if (s_NavMeshSurfaces.Count == 0)
            {
                NavMesh.onPreUpdate -= UpdateActive;
            }
        }

        private static void UpdateActive()
        {
            for (int i = 0; i < s_NavMeshSurfaces.Count; ++i)
            {
                s_NavMeshSurfaces[i].UpdateDataIfTransformChanged();
            }
        }

        private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
#if UNITY_EDITOR
            StageHandle myStage = StageUtility.GetStageHandle(gameObject);
            if (!myStage.IsValid())
            {
                return;
            }
#endif
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (m_CollectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifierVolume>(
                    GetComponentsInChildren<NavMeshModifierVolume>()
                );
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifierVolume.activeModifiers;
            }

            foreach (NavMeshModifierVolume m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                {
                    continue;
                }

                if (!m.AffectsAgentType(m_AgentTypeID))
                {
                    continue;
                }
#if UNITY_EDITOR
                if (!myStage.Contains(m.gameObject))
                {
                    continue;
                }
#endif
                Vector3 mcenter = m.transform.TransformPoint(m.center);
                Vector3 scale = m.transform.lossyScale;
                Vector3 msize =
                    new(
                        m.size.x * Mathf.Abs(scale.x),
                        m.size.y * Mathf.Abs(scale.y),
                        m.size.z * Mathf.Abs(scale.z)
                    );

                NavMeshBuildSource src =
                    new()
                    {
                        shape = NavMeshBuildSourceShape.ModifierBox,
                        transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one),
                        size = msize,
                        area = m.area
                    };
                sources.Add(src);
            }
        }

        private List<NavMeshBuildSource> CollectSources()
        {
            List<NavMeshBuildSource> sources = new();
            List<NavMeshBuildMarkup> markups = new();

            List<NavMeshModifier> modifiers;
            if (m_CollectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifier.activeModifiers;
            }

            foreach (NavMeshModifier m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                {
                    continue;
                }

                if (!m.AffectsAgentType(m_AgentTypeID))
                {
                    continue;
                }

                NavMeshBuildMarkup markup =
                    new()
                    {
                        root = m.transform,
                        overrideArea = m.overrideArea,
                        area = m.area,
                        ignoreFromBuild = m.ignoreFromBuild
                    };
                markups.Add(markup);
            }

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (m_CollectObjects == CollectObjects.All)
                {
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        null,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        gameObject.scene,
                        sources
                    );
                }
                else if (m_CollectObjects == CollectObjects.Children)
                {
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        transform,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        gameObject.scene,
                        sources
                    );
                }
                else if (m_CollectObjects == CollectObjects.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(
                        transform.position,
                        transform.rotation,
                        Vector3.one
                    );
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(m_Center, m_Size));

                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        worldBounds,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        gameObject.scene,
                        sources
                    );
                }
            }
            else
#endif
            {
                if (m_CollectObjects == CollectObjects.All)
                {
                    NavMeshBuilder.CollectSources(
                        null,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        sources
                    );
                }
                else if (m_CollectObjects == CollectObjects.Children)
                {
                    NavMeshBuilder.CollectSources(
                        transform,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        sources
                    );
                }
                else if (m_CollectObjects == CollectObjects.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(
                        transform.position,
                        transform.rotation,
                        Vector3.one
                    );
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(m_Center, m_Size));
                    NavMeshBuilder.CollectSources(
                        worldBounds,
                        m_LayerMask,
                        m_UseGeometry,
                        m_DefaultArea,
                        markups,
                        sources
                    );
                }
            }

            if (m_IgnoreNavMeshAgent)
            {
                sources.RemoveAll(
                    (x) =>
                        x.component != null
                        && x.component.gameObject.GetComponent<NavMeshAgent>() != null
                );
            }

            if (m_IgnoreNavMeshObstacle)
            {
                sources.RemoveAll(
                    (x) =>
                        x.component != null
                        && x.component.gameObject.GetComponent<NavMeshObstacle>() != null
                );
            }

            AppendModifierVolumes(ref sources);

            return sources;
        }

        private static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        private static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            Vector3 absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            Vector3 absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            Vector3 absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            Vector3 worldPosition = mat.MultiplyPoint(bounds.center);
            Vector3 worldSize =
                (absAxisX * bounds.size.x)
                + (absAxisY * bounds.size.y)
                + (absAxisZ * bounds.size.z);
            return new Bounds(worldPosition, worldSize);
        }

        private Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
        {
            // Use the unscaled matrix for the NavMeshSurface
            Matrix4x4 worldToLocal = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                Vector3.one
            );
            worldToLocal = worldToLocal.inverse;

            Bounds result = new();
            foreach (NavMeshBuildSource src in sources)
            {
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                    {
                        Mesh m = src.sourceObject as Mesh;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                        break;
                    }
                    case NavMeshBuildSourceShape.Terrain:
                    {
                        // Terrain pivot is lower/left corner - shift bounds accordingly
                        TerrainData t = src.sourceObject as TerrainData;
                        result.Encapsulate(
                            GetWorldBounds(
                                worldToLocal * src.transform,
                                new Bounds(0.5f * t.size, t.size)
                            )
                        );
                        break;
                    }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(
                            GetWorldBounds(
                                worldToLocal * src.transform,
                                new Bounds(Vector3.zero, src.size)
                            )
                        );
                        break;
                }
            }
            // Inflate the bounds a bit to avoid clipping co-planar sources
            result.Expand(0.1f);
            return result;
        }

        private bool HasTransformChanged()
        {
            return m_LastPosition != transform.position || m_LastRotation != transform.rotation;
        }

        private void UpdateDataIfTransformChanged()
        {
            if (HasTransformChanged())
            {
                RemoveData();
                AddData();
            }
        }

#if UNITY_EDITOR
        private bool UnshareNavMeshAsset()
        {
            // Nothing to unshare
            if (m_NavMeshData == null)
            {
                return false;
            }

            // Prefab parent owns the asset reference
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPersistentObject = EditorUtility.IsPersistent(this);
            if (isInPreviewScene || isPersistentObject)
            {
                return false;
            }

            // An instance can share asset reference only with its prefab parent
            NavMeshSurface prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(
                this
            );
            if (prefab != null && prefab.navMeshData == navMeshData)
            {
                return false;
            }

            // Don't allow referencing an asset that's assigned to another surface
            for (int i = 0; i < s_NavMeshSurfaces.Count; ++i)
            {
                NavMeshSurface surface = s_NavMeshSurfaces[i];
                if (surface != this && surface.m_NavMeshData == m_NavMeshData)
                {
                    return true;
                }
            }

            // Asset is not referenced by known surfaces
            return false;
        }

        private void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                Debug.LogWarning(
                    "Duplicating NavMeshSurface does not duplicate the referenced navmesh data",
                    this
                );
                m_NavMeshData = null;
            }

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (settings.agentTypeID != -1)
            {
                // When unchecking the override control, revert to automatic value.
                const float kMinVoxelSize = 0.01f;
                if (!m_OverrideVoxelSize)
                {
                    m_VoxelSize = settings.agentRadius / 3.0f;
                }

                if (m_VoxelSize < kMinVoxelSize)
                {
                    m_VoxelSize = kMinVoxelSize;
                }

                // When unchecking the override control, revert to default value.
                const int kMinTileSize = 16;
                const int kMaxTileSize = 1024;
                const int kDefaultTileSize = 256;

                if (!m_OverrideTileSize)
                {
                    m_TileSize = kDefaultTileSize;
                }
                // Make sure tilesize is in sane range.
                if (m_TileSize < kMinTileSize)
                {
                    m_TileSize = kMinTileSize;
                }

                if (m_TileSize > kMaxTileSize)
                {
                    m_TileSize = kMaxTileSize;
                }
            }
        }
#endif
    }
}