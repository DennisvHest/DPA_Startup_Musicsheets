using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PSAMWPFControlLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DPA_Musicsheets.Hotkeys.KeyHandlers;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.ViewModels {
    public class MainViewModel : ViewModelBase
    {
        private readonly ISet<Key> _pressedKeys;
        private readonly IKeyHandlerChain _keyHandlerChain;

        private string _fileName;
        public string FileName {
            get {
                return _fileName;
            }
            set {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        /// <summary>
        /// The current state can be used to display some text.
        /// "Rendering..." is a text that will be displayed for example.
        /// </summary>
        private EditorState _currentState;
        public EditorState CurrentState {
            get { return _currentState; }
            set { _currentState = value; RaisePropertyChanged(() => CurrentState); }
        }

        private MusicLoader _musicLoader;

        public MainViewModel(MusicLoader musicLoader) {
            CurrentState = new Saved(this);

            _pressedKeys = new SortedSet<Key>();

            _keyHandlerChain = new CommandKeyHandler();
            RegularKeyHandler keyHandlerChain2 = new RegularKeyHandler();
            RegularKeyHandler keyHandlerChain3 = new RegularKeyHandler();
            _keyHandlerChain.Next = keyHandlerChain2;
            keyHandlerChain2.Next = keyHandlerChain3;

            _musicLoader = musicLoader;
            FileName = @"Files/Alle-eendjes-zwemmen-in-het-water.mid";
        }

        public ICommand OpenFileCommand => new RelayCommand(() => {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            if (openFileDialog.ShowDialog() == true) {
                FileName = openFileDialog.FileName;
            }
        });

        public ICommand LoadCommand => new RelayCommand(() => {
            _musicLoader.OpenFile(FileName);
        });

        #region Focus and key commands, these can be used for implementing hotkeys
        public ICommand OnLostFocusCommand => new RelayCommand(() => {
            Console.WriteLine("Maingrid Lost focus");
        });

        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) => {
            _pressedKeys.Add(e.Key);

            _keyHandlerChain.Handle(new List<Key>(_pressedKeys));
        });

        public ICommand OnKeyUpCommand => new RelayCommand<KeyEventArgs>((e) => {
            _pressedKeys.Remove(e.Key);
        });

        public ICommand OnWindowClosingCommand => new RelayCommand<CancelEventArgs>((e) => {
            if (!CurrentState.CanQuit) {
                MessageBoxResult result = MessageBox.Show("Weet je zeker dat je wilt afsluiten? Onopgeslagen wijzigingen zullen verloren raken.",
                    "Er zijn nog onopgeslagen gegevens.", MessageBoxButton.YesNo);

                e.Cancel = result != MessageBoxResult.Yes;
            }

            if (!e.Cancel)
                ViewModelLocator.Cleanup();
        });
        #endregion Focus and key commands, these can be used for implementing hotkeys
    }
}
