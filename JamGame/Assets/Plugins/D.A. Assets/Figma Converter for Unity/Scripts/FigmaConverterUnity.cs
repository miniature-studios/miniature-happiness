//
//███████╗██╗░██████╗░███╗░░░███╗░█████╗░  ░█████╗░░█████╗░███╗░░██╗██╗░░░██╗███████╗██████╗░████████╗███████╗██████╗░
//██╔════╝██║██╔════╝░████╗░████║██╔══██╗  ██╔══██╗██╔══██╗████╗░██║██║░░░██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝██╔══██╗
//█████╗░░██║██║░░██╗░██╔████╔██║███████║  ██║░░╚═╝██║░░██║██╔██╗██║╚██╗░██╔╝█████╗░░██████╔╝░░░██║░░░█████╗░░██████╔╝
//██╔══╝░░██║██║░░╚██╗██║╚██╔╝██║██╔══██║  ██║░░██╗██║░░██║██║╚████║░╚████╔╝░██╔══╝░░██╔══██╗░░░██║░░░██╔══╝░░██╔══██╗
//██║░░░░░██║╚██████╔╝██║░╚═╝░██║██║░░██║  ╚█████╔╝╚█████╔╝██║░╚███║░░╚██╔╝░░███████╗██║░░██║░░░██║░░░███████╗██║░░██║
//╚═╝░░░░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝  ░╚════╝░░╚════╝░╚═╝░░╚══╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝
//
//███████╗░█████╗░██████╗░  ██╗░░░██╗███╗░░██╗██╗████████╗██╗░░░██╗
//██╔════╝██╔══██╗██╔══██╗  ██║░░░██║████╗░██║██║╚══██╔══╝╚██╗░██╔╝
//█████╗░░██║░░██║██████╔╝  ██║░░░██║██╔██╗██║██║░░░██║░░░░╚████╔╝░
//██╔══╝░░██║░░██║██╔══██╗  ██║░░░██║██║╚████║██║░░░██║░░░░░╚██╔╝░░
//██║░░░░░╚█████╔╝██║░░██║  ╚██████╔╝██║░╚███║██║░░░██║░░░░░░██║░░░
//╚═╝░░░░░░╚════╝░╚═╝░░╚═╝  ░╚═════╝░╚═╝░░╚══╝╚═╝░░░╚═╝░░░░░░╚═╝░░░
//
using DA_Assets.FCU.Drawers;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public sealed class FigmaConverterUnity : MonoBehaviour
    {
        [SerializeField] ProjectImporter importController;
        [SerializeProperty(nameof(importController))]
        public ProjectImporter ProjectImporter => importController.SetMonoBehaviour(this);

        [SerializeField] CanvasDrawer canvasDrawer;
        [SerializeProperty(nameof(canvasDrawer))]
        public CanvasDrawer CanvasDrawer => canvasDrawer.SetMonoBehaviour(this);

#if UITKPLUGIN_EXISTS
        [SerializeField] UITK_Converter uitkConverter;
        [SerializeProperty(nameof(uitkConverter))]
        public UITK_Converter UITK_Converter => uitkConverter.SetMonoBehaviour(this);
#endif
        [SerializeField] ProjectCacher projectCacher;
        [SerializeProperty(nameof(projectCacher))]
        public ProjectCacher ProjectCacher => projectCacher.SetMonoBehaviour(this);

        [SerializeField] HashCacher hashCacher;
        [SerializeProperty(nameof(hashCacher))]
        public HashCacher HashCacher => hashCacher.SetMonoBehaviour(this);

        [SerializeField] ProjectDownloader projectDownloader;
        [SerializeProperty(nameof(projectDownloader))]
        public ProjectDownloader ProjectDownloader => projectDownloader.SetMonoBehaviour(this);

        [SerializeField] ImageTypeSetter imageTypeSetter;
        [SerializeProperty(nameof(imageTypeSetter))]
        public ImageTypeSetter ImageTypeSetter => imageTypeSetter.SetMonoBehaviour(this);

        [SerializeField] DuplicateFinder duplicateFinder;
        [SerializeProperty(nameof(duplicateFinder))]
        public DuplicateFinder DuplicateFinder => duplicateFinder.SetMonoBehaviour(this);

        [SerializeField] DelegateHolder delegateHolder;
        [SerializeProperty(nameof(delegateHolder))]
        public DelegateHolder DelegateHolder => delegateHolder.SetMonoBehaviour(this);

        [SerializeField] SettingsBinder settings;
        [SerializeProperty(nameof(settings))]
        public SettingsBinder Settings => settings.SetMonoBehaviour(this);

        [SerializeField] FcuEventHandlers eventHandlers;
        [SerializeProperty(nameof(eventHandlers))]
        public FcuEventHandlers EventHandlers => eventHandlers.SetMonoBehaviour(this);

        [SerializeField] FcuEvents events;
        [SerializeProperty(nameof(events))]
        public FcuEvents Events => events.SetMonoBehaviour(this);

        [SerializeField] PrefabCreator prefabCreator;
        [SerializeProperty(nameof(prefabCreator))]
        public PrefabCreator PrefabCreator => prefabCreator.SetMonoBehaviour(this);

        [SerializeField] InspectorDrawer inspectorDrawer;
        [SerializeProperty(nameof(inspectorDrawer))]
        public InspectorDrawer InspectorDrawer => inspectorDrawer.SetMonoBehaviour(this);

        [SerializeField] Authorizer authController;
        [SerializeProperty(nameof(authController))]
        public Authorizer AuthController => authController.SetMonoBehaviour(this);

        [SerializeField] RequestSender requestSender;
        [SerializeProperty(nameof(requestSender))]
        public RequestSender RequestSender => requestSender.SetMonoBehaviour(this);

        [SerializeField] HashGenerator hashGenerator;
        [SerializeProperty(nameof(hashGenerator))]
        public HashGenerator HashGenerator => hashGenerator.SetMonoBehaviour(this);

        [SerializeField] NameHumanizer nameHumanizer;
        [SerializeProperty(nameof(nameHumanizer))]
        public NameHumanizer NameHumanizer => nameHumanizer.SetMonoBehaviour(this);

        [SerializeField] FontDownloader fontDownloader;
        [SerializeProperty(nameof(fontDownloader))]
        public FontDownloader FontDownloader => fontDownloader.SetMonoBehaviour(this);

        [SerializeField] FontLoader fontLoader;
        [SerializeProperty(nameof(fontLoader))]
        public FontLoader FontLoader => fontLoader.SetMonoBehaviour(this);

        [SerializeField] TagSetter tagSetter;
        [SerializeProperty(nameof(tagSetter))]
        public TagSetter TagSetter => tagSetter.SetMonoBehaviour(this);

        [SerializeField] SpriteWorker spriteWorker;
        [SerializeProperty(nameof(spriteWorker))]
        public SpriteWorker SpriteWorker => spriteWorker.SetMonoBehaviour(this);

        [SerializeField] AssetTools tools;
        [SerializeProperty(nameof(tools))]
        public AssetTools AssetTools => tools.SetMonoBehaviour(this);

        [SerializeField] SyncHelpers syncHelper;
        [SerializeProperty(nameof(syncHelper))]
        public SyncHelpers SyncHelpers => syncHelper.SetMonoBehaviour(this);

        [SerializeField] TransformSetter transformSetter;
        [SerializeProperty(nameof(transformSetter))]
        public TransformSetter TransformSetter => transformSetter.SetMonoBehaviour(this);

        [SerializeField] CurrentProject currentProject;
        [SerializeProperty(nameof(currentProject))]
        public CurrentProject CurrentProject => currentProject.SetMonoBehaviour(this);

        [SerializeField] SpriteGenerator spriteGenerator;
        [SerializeProperty(nameof(spriteGenerator))]
        public SpriteGenerator SpriteGenerator => spriteGenerator.SetMonoBehaviour(this);

        [SerializeField] SpriteColorizer spriteColorizer;
        [SerializeProperty(nameof(spriteColorizer))]
        public SpriteColorizer SpriteColorizer => spriteColorizer.SetMonoBehaviour(this);

        [SerializeField] SpritePathSetter spritePathSetter;
        [SerializeProperty(nameof(spritePathSetter))]
        public SpritePathSetter SpritePathSetter => spritePathSetter.SetMonoBehaviour(this);

        [SerializeField] SpriteDownloader spriteDownloader;
        [SerializeProperty(nameof(spriteDownloader))]
        public SpriteDownloader SpriteDownloader => spriteDownloader.SetMonoBehaviour(this);

        [SerializeField] FigmaSession figmaSession;
        [SerializeProperty(nameof(figmaSession))]
        public FigmaSession FigmaSession => figmaSession.SetMonoBehaviour(this);

        [SerializeField] string guid;
        public string Guid => guid.CreateShortGuid(out guid);
    }
}