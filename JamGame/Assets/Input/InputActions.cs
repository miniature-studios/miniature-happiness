//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Input/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""CameraLook"",
            ""id"": ""cab3f0b9-a29e-4f63-a694-92a876e53e9c"",
            ""actions"": [
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""e6c6ab81-1465-4dd8-8b91-970f8da1b56d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Clamp(min=-1,max=1),Invert"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""fd76cf1c-6016-48e7-b432-c85daeefbdf1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""0301901e-9327-43d8-b523-e518dd16af6b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Clamp(min=-1,max=1)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""610e1cdf-e94c-44bd-8ce9-c649176e8f32"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false),ScaleVector2(x=2,y=2)"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d366cc48-37bc-4a43-820d-655750955baf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ca5f2758-db8b-4fba-8c8b-9ecf2b93b648"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""00ad0481-b6c5-4282-90f0-8e0a4f908907"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bbe10461-da64-47a3-a372-f7d0c58ecba4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse move"",
                    ""id"": ""cca7757e-d290-4f6f-ae78-ab5e7de87376"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f86f9beb-ee30-47d2-b178-078963f341a8"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""4b57905c-3b7d-43e2-bac7-cfe30e0d03cb"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertY=false)"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""QE"",
                    ""id"": ""f6cd3761-6aca-4140-986d-9884ec24e84f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""23b66f3e-0d88-4dc1-88f2-dbe2d7dafc88"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""871a739a-9221-4dc5-8f30-183068941fe5"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse Rotate"",
                    ""id"": ""bcba3cd9-677e-4acc-b9f6-d4f641046e31"",
                    ""path"": ""TwoModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier1"",
                    ""id"": ""943157a6-901e-4c75-a19f-141ab0049ea4"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier2"",
                    ""id"": ""04267760-4df1-44e7-9946-fee8d616b67b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""7674ef61-4576-46f3-b56c-5a396d08d81d"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Middle mouse button"",
                    ""id"": ""5f12fe83-258b-4a55-95f5-f7cf41a706cc"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""3316db90-7489-4b2a-a1bb-333286d2c070"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""3f1b4550-2bbe-4313-bb7e-0f91a203a50b"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5a34a13c-149c-442e-9dd2-9d0580f8fff7"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""34da3c5c-c727-4b90-81d9-d0de36e9b1a3"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""ca00c751-63df-4f90-962f-942b22670f07"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateTile"",
                    ""type"": ""Button"",
                    ""id"": ""eac92c0d-6b2e-42c2-936a-1ec07e7895a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ExtendInventoryTileInfo"",
                    ""type"": ""Button"",
                    ""id"": ""01da7ed3-c0c9-447c-9a7d-3baf850d8523"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dc4f8cb0-5849-42f5-9c86-c148e2fbad87"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35141c51-0a7b-4e2e-9fd4-9d94e164132f"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateTile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a5a198e-8493-4aff-b01d-f3338b6623d0"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExtendInventoryTileInfo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CameraLook
        m_CameraLook = asset.FindActionMap("CameraLook", throwIfNotFound: true);
        m_CameraLook_Zoom = m_CameraLook.FindAction("Zoom", throwIfNotFound: true);
        m_CameraLook_Move = m_CameraLook.FindAction("Move", throwIfNotFound: true);
        m_CameraLook_Rotate = m_CameraLook.FindAction("Rotate", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Pause = m_UI.FindAction("Pause", throwIfNotFound: true);
        m_UI_RotateTile = m_UI.FindAction("RotateTile", throwIfNotFound: true);
        m_UI_ExtendInventoryTileInfo = m_UI.FindAction("ExtendInventoryTileInfo", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // CameraLook
    private readonly InputActionMap m_CameraLook;
    private List<ICameraLookActions> m_CameraLookActionsCallbackInterfaces = new List<ICameraLookActions>();
    private readonly InputAction m_CameraLook_Zoom;
    private readonly InputAction m_CameraLook_Move;
    private readonly InputAction m_CameraLook_Rotate;
    public struct CameraLookActions
    {
        private @InputActions m_Wrapper;
        public CameraLookActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Zoom => m_Wrapper.m_CameraLook_Zoom;
        public InputAction @Move => m_Wrapper.m_CameraLook_Move;
        public InputAction @Rotate => m_Wrapper.m_CameraLook_Rotate;
        public InputActionMap Get() { return m_Wrapper.m_CameraLook; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraLookActions set) { return set.Get(); }
        public void AddCallbacks(ICameraLookActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraLookActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraLookActionsCallbackInterfaces.Add(instance);
            @Zoom.started += instance.OnZoom;
            @Zoom.performed += instance.OnZoom;
            @Zoom.canceled += instance.OnZoom;
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Rotate.started += instance.OnRotate;
            @Rotate.performed += instance.OnRotate;
            @Rotate.canceled += instance.OnRotate;
        }

        private void UnregisterCallbacks(ICameraLookActions instance)
        {
            @Zoom.started -= instance.OnZoom;
            @Zoom.performed -= instance.OnZoom;
            @Zoom.canceled -= instance.OnZoom;
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Rotate.started -= instance.OnRotate;
            @Rotate.performed -= instance.OnRotate;
            @Rotate.canceled -= instance.OnRotate;
        }

        public void RemoveCallbacks(ICameraLookActions instance)
        {
            if (m_Wrapper.m_CameraLookActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraLookActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraLookActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraLookActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraLookActions @CameraLook => new CameraLookActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_Pause;
    private readonly InputAction m_UI_RotateTile;
    private readonly InputAction m_UI_ExtendInventoryTileInfo;
    public struct UIActions
    {
        private @InputActions m_Wrapper;
        public UIActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_UI_Pause;
        public InputAction @RotateTile => m_Wrapper.m_UI_RotateTile;
        public InputAction @ExtendInventoryTileInfo => m_Wrapper.m_UI_ExtendInventoryTileInfo;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @RotateTile.started += instance.OnRotateTile;
            @RotateTile.performed += instance.OnRotateTile;
            @RotateTile.canceled += instance.OnRotateTile;
            @ExtendInventoryTileInfo.started += instance.OnExtendInventoryTileInfo;
            @ExtendInventoryTileInfo.performed += instance.OnExtendInventoryTileInfo;
            @ExtendInventoryTileInfo.canceled += instance.OnExtendInventoryTileInfo;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @RotateTile.started -= instance.OnRotateTile;
            @RotateTile.performed -= instance.OnRotateTile;
            @RotateTile.canceled -= instance.OnRotateTile;
            @ExtendInventoryTileInfo.started -= instance.OnExtendInventoryTileInfo;
            @ExtendInventoryTileInfo.performed -= instance.OnExtendInventoryTileInfo;
            @ExtendInventoryTileInfo.canceled -= instance.OnExtendInventoryTileInfo;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface ICameraLookActions
    {
        void OnZoom(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnPause(InputAction.CallbackContext context);
        void OnRotateTile(InputAction.CallbackContext context);
        void OnExtendInventoryTileInfo(InputAction.CallbackContext context);
    }
}
