// GENERATED AUTOMATICALLY FROM 'Assets/Input System/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""7461136e-a03b-4624-b68c-76c0146f6d7c"",
            ""actions"": [
                {
                    ""name"": ""Move & Aim"",
                    ""type"": ""Button"",
                    ""id"": ""bce2cfd5-0189-47eb-b321-b8e58822fef2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d0028c6e-5ed8-4121-9db9-2d39b6c8fcda"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Parachute"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b51101f8-bcde-43bf-a872-870411dc6340"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeSpecialItem"",
                    ""type"": ""Value"",
                    ""id"": ""6facd9bd-8bf1-47c7-811f-f5f33c9273e9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TimeBombMovement"",
                    ""type"": ""Button"",
                    ""id"": ""fcbfdd7b-f7ce-40ee-9a4d-bf469386fa2b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire1 Shot"",
                    ""type"": ""Button"",
                    ""id"": ""26902dcd-2ac0-4c77-ac63-21d2e4c18cbe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire2 Shot"",
                    ""type"": ""Button"",
                    ""id"": ""ca781ac6-61c2-4f7e-9564-b4ee45a60eef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire3 SpecialItem"",
                    ""type"": ""Button"",
                    ""id"": ""82eed940-98df-4c06-b006-f98c5eee2d16"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""aa92b8b9-485e-4d5e-9969-d04f2b7d88cc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ebffc62-cec2-4ad6-bbf5-e7c14e3fffd4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Gamepad RL"",
                    ""id"": ""c9c6b616-96ca-431b-bb45-51fc1438be03"",
                    ""path"": ""1DAxis(whichSideWins=1)"",
                    ""interactions"": """",
                    ""processors"": ""Normalize(min=-1,max=1),Scale"",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""177a4fb7-ed56-4675-a99b-a3ee5c4631b9"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""50e978a7-03e3-43e1-80ad-eaeac69b7f12"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""a8490052-66da-4db8-a316-30fd255010ae"",
                    ""path"": ""1DAxis(whichSideWins=1)"",
                    ""interactions"": """",
                    ""processors"": ""Normalize(min=-1,max=1),Scale"",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""2d5a6264-4673-4e41-8d63-4728bc6a225d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f4aa60ce-b467-4db1-bd89-ae57e923b55c"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeSpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ec11ffa6-4f51-41b8-8198-314ebb05a1a6"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeBombMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ef40eca7-1053-440e-9725-bc58d5bdf81f"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeBombMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8f3247e4-29df-4741-b91a-f704fccf1531"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeBombMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""D Pad"",
                    ""id"": ""99fa2ced-b246-4e09-839e-36b495a8ea7c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e31fe8cd-13d2-4d65-aa34-c7e4115e5a96"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4c79bc99-d39f-4610-b5d3-800df746d1bf"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c06c9e65-18f6-481a-be3c-4a55175688f3"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f067712e-b6cc-4bd4-86c6-585b2d1783e1"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""df6b32c9-e3d7-4dd2-ac67-a71add953575"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""70daf97d-acdd-4a16-9444-d4cc0bb08c0b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""33aebc7a-6683-4c0c-97b4-15c9657097d2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""159a2c8a-382c-488c-9ce8-89411b70dbc2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""90c51b14-1da5-4401-98a8-723e885119b2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move & Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1dd5d6f1-a3a7-4e14-8fe1-6a2716284d6f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Parachute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d60f617a-3786-431b-bcd0-e9e174b1e8ce"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Parachute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3734d4b6-837e-4a6e-9d6d-46f1b60c9049"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire1 Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc3c3b4b-5452-4c9c-bec2-bd78cc5c929c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire1 Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a01a853-aee5-4d48-98f4-15ffb3e9ca91"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire2 Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""121743c4-892d-4e9a-927e-fa404ab0ce96"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire2 Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc3959a2-3024-4d55-94b0-8afabf7a0929"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire3 SpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85b8882f-ccfb-4b5c-b985-aa2080d52a20"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire3 SpecialItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Joystick"",
            ""bindingGroup"": ""Keyboard & Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MoveAim = m_Player.FindAction("Move & Aim", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Parachute = m_Player.FindAction("Parachute", throwIfNotFound: true);
        m_Player_ChangeSpecialItem = m_Player.FindAction("ChangeSpecialItem", throwIfNotFound: true);
        m_Player_TimeBombMovement = m_Player.FindAction("TimeBombMovement", throwIfNotFound: true);
        m_Player_Fire1Shot = m_Player.FindAction("Fire1 Shot", throwIfNotFound: true);
        m_Player_Fire2Shot = m_Player.FindAction("Fire2 Shot", throwIfNotFound: true);
        m_Player_Fire3SpecialItem = m_Player.FindAction("Fire3 SpecialItem", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MoveAim;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Parachute;
    private readonly InputAction m_Player_ChangeSpecialItem;
    private readonly InputAction m_Player_TimeBombMovement;
    private readonly InputAction m_Player_Fire1Shot;
    private readonly InputAction m_Player_Fire2Shot;
    private readonly InputAction m_Player_Fire3SpecialItem;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveAim => m_Wrapper.m_Player_MoveAim;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Parachute => m_Wrapper.m_Player_Parachute;
        public InputAction @ChangeSpecialItem => m_Wrapper.m_Player_ChangeSpecialItem;
        public InputAction @TimeBombMovement => m_Wrapper.m_Player_TimeBombMovement;
        public InputAction @Fire1Shot => m_Wrapper.m_Player_Fire1Shot;
        public InputAction @Fire2Shot => m_Wrapper.m_Player_Fire2Shot;
        public InputAction @Fire3SpecialItem => m_Wrapper.m_Player_Fire3SpecialItem;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MoveAim.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveAim;
                @MoveAim.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveAim;
                @MoveAim.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveAim;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Parachute.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnParachute;
                @Parachute.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnParachute;
                @Parachute.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnParachute;
                @ChangeSpecialItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSpecialItem;
                @ChangeSpecialItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSpecialItem;
                @ChangeSpecialItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSpecialItem;
                @TimeBombMovement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTimeBombMovement;
                @TimeBombMovement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTimeBombMovement;
                @TimeBombMovement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTimeBombMovement;
                @Fire1Shot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire1Shot;
                @Fire1Shot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire1Shot;
                @Fire1Shot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire1Shot;
                @Fire2Shot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire2Shot;
                @Fire2Shot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire2Shot;
                @Fire2Shot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire2Shot;
                @Fire3SpecialItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire3SpecialItem;
                @Fire3SpecialItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire3SpecialItem;
                @Fire3SpecialItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire3SpecialItem;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveAim.started += instance.OnMoveAim;
                @MoveAim.performed += instance.OnMoveAim;
                @MoveAim.canceled += instance.OnMoveAim;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Parachute.started += instance.OnParachute;
                @Parachute.performed += instance.OnParachute;
                @Parachute.canceled += instance.OnParachute;
                @ChangeSpecialItem.started += instance.OnChangeSpecialItem;
                @ChangeSpecialItem.performed += instance.OnChangeSpecialItem;
                @ChangeSpecialItem.canceled += instance.OnChangeSpecialItem;
                @TimeBombMovement.started += instance.OnTimeBombMovement;
                @TimeBombMovement.performed += instance.OnTimeBombMovement;
                @TimeBombMovement.canceled += instance.OnTimeBombMovement;
                @Fire1Shot.started += instance.OnFire1Shot;
                @Fire1Shot.performed += instance.OnFire1Shot;
                @Fire1Shot.canceled += instance.OnFire1Shot;
                @Fire2Shot.started += instance.OnFire2Shot;
                @Fire2Shot.performed += instance.OnFire2Shot;
                @Fire2Shot.canceled += instance.OnFire2Shot;
                @Fire3SpecialItem.started += instance.OnFire3SpecialItem;
                @Fire3SpecialItem.performed += instance.OnFire3SpecialItem;
                @Fire3SpecialItem.canceled += instance.OnFire3SpecialItem;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardJoystickSchemeIndex = -1;
    public InputControlScheme KeyboardJoystickScheme
    {
        get
        {
            if (m_KeyboardJoystickSchemeIndex == -1) m_KeyboardJoystickSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Joystick");
            return asset.controlSchemes[m_KeyboardJoystickSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMoveAim(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnParachute(InputAction.CallbackContext context);
        void OnChangeSpecialItem(InputAction.CallbackContext context);
        void OnTimeBombMovement(InputAction.CallbackContext context);
        void OnFire1Shot(InputAction.CallbackContext context);
        void OnFire2Shot(InputAction.CallbackContext context);
        void OnFire3SpecialItem(InputAction.CallbackContext context);
    }
}
