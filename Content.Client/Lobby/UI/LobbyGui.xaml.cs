using Content.Client.Message;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Lobby.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class LobbyGui : UIScreen
    {
        [Dependency] private readonly IClientConsoleHost _consoleHost = default!;

        public LobbyGui()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);
            SetAnchorPreset(MainContainer, LayoutPreset.Wide);
            SetAnchorPreset(Background, LayoutPreset.Wide);
            SetAnchorPreset(ShowInterfaceContainer, LayoutPreset.Wide); // ADT-Tweak
            SetAnchorPreset(ShowInterface, LayoutPreset.BottomLeft); // ADT-Tweak

            LobbySong.SetMarkup(Loc.GetString("lobby-state-song-no-song-text"));

            LeaveButton.OnPressed += _ => _consoleHost.ExecuteCommand("disconnect");
            OptionsButton.OnPressed += _ => UserInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();
            // ADT-Tweak-Start
            HideInterface.OnPressed += _ => {
                SwitchState(LobbyGuiState.ScreenSaver);
            };
            ShowInterface.OnPressed += _ => {
                SwitchState(LobbyGuiState.Default);
            };
            // ADT-Tweak-End
        }

        public void SwitchState(LobbyGuiState state)
        {
            DefaultState.Visible = false;
            CharacterSetupState.Visible = false;
            // ADT-Tweak-Start
            ShowInterfaceContainer.Visible = false;
            MainContainer.Visible = true;
            // ADT-Tweak-End

            switch (state)
            {
                case LobbyGuiState.Default:
                    DefaultState.Visible = true;
                    RightSide.Visible = true;
                    break;
                case LobbyGuiState.CharacterSetup:
                    CharacterSetupState.Visible = true;

                    var actualWidth = (float) UserInterfaceManager.RootControl.PixelWidth;
                    var setupWidth = (float) LeftSide.PixelWidth;

                    if (1 - (setupWidth / actualWidth) > 0.30)
                    {
                        RightSide.Visible = false;
                    }

                    UserInterfaceManager.GetUIController<LobbyUIController>().ReloadCharacterSetup();

                    break;

                // ADT-Tweak-Start
                case LobbyGuiState.ScreenSaver:
                    ShowInterfaceContainer.Visible = true;
                    MainContainer.Visible = false;
                    break;
                // ADT-Tweak-End
            }
        }

        public enum LobbyGuiState : byte
        {
            /// <summary>
            ///  The default state, i.e., what's seen on launch.
            /// </summary>
            Default,
            /// <summary>
            ///  The character setup state.
            /// </summary>
            CharacterSetup,
            ScreenSaver, // ADT-Tweak
        }
    }
}
