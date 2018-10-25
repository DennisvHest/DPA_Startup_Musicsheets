using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {
        private MusicLoader _musicLoader;
        private MainViewModel _mainViewModel { get; set; }

        private readonly LilypondOriginator _lilypondOriginator;
        private readonly Stack<LilypondMemento> _undoMementos;
        private readonly Stack<LilypondMemento> _redoMementos;
        private string _text;
        private string _previousText;
        private string _nextText;

        /// <summary>
        /// This text will be in the textbox.
        /// It can be filled either by typing or loading a file so we only want to set previoustext when it's caused by typing.
        /// </summary>
        public string LilypondText
        {
            get => _text;
            set
            {
                _lilypondOriginator.Text = _text;
                if (!_waitingForRender && !_textChangedByLoad)
                {
                    _redoMementos.Clear();
                    _undoMementos.Push(_lilypondOriginator.Save());
                }

                _text = value;

                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private DateTime _lastChange;
        private static int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private bool _waitingForRender = false;

        public LilypondViewModel(MainViewModel mainViewModel, MusicLoader musicLoader)
        {
            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            _mainViewModel = mainViewModel;
            _musicLoader = musicLoader;
            _musicLoader.LilypondViewModel = this;
            
            _text = "Your lilypond text will appear here.";

            _lilypondOriginator = new LilypondOriginator();
            _undoMementos = new Stack<LilypondMemento>();
            _redoMementos = new Stack<LilypondMemento>();
        }

        public void LilypondTextLoaded(string text)
        {
            _undoMementos.Clear();
            _redoMementos.Clear();
            _textChangedByLoad = true;
            LilypondText = _previousText = text;
            _textChangedByLoad = false;
        }

        /// <summary>
        /// This occurs when the text in the textbox has changed. This can either be by loading or typing.
        /// </summary>
        public ICommand TextChangedCommand => new RelayCommand<TextChangedEventArgs>((args) =>
        {
            // If we were typing, we need to do things.
            if (!_textChangedByLoad)
            {
                _waitingForRender = true;
                _lastChange = DateTime.Now;

                _mainViewModel.CurrentState.TextChanged();

                Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
                {
                    if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                    {
                        _waitingForRender = false;
                        _mainViewModel.CurrentState.RenderingFinished();
                        UndoCommand.RaiseCanExecuteChanged();

                        _musicLoader.LoadLilyPond(LilypondText);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });

        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            // Add current state to the redo mementos
            _lilypondOriginator.Text = _text;
            _redoMementos.Push(_lilypondOriginator.Save());

            // Restore state from the undo mementos
            LilypondMemento memento = _undoMementos.Pop();
            _lilypondOriginator.Restore(memento);
            _text = _lilypondOriginator.Text;
            RaisePropertyChanged(() => LilypondText);
        }, () => _undoMementos.Any());

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            // Add current state to the undo mementos
            _lilypondOriginator.Text = _text;
            _undoMementos.Push(_lilypondOriginator.Save());

            // Restore state from the redo mementos
            LilypondMemento memento = _redoMementos.Pop();
            _lilypondOriginator.Restore(memento);
            _text = _lilypondOriginator.Text;
            RaisePropertyChanged(() => LilypondText);
        }, () => _redoMementos.Any());

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            // TODO: In the application a lot of classes know which filetypes are supported. Lots and lots of repeated code here...
            // Can this be done better?
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension.EndsWith(".mid"))
                {
                    _musicLoader.SaveToMidi(saveFileDialog.FileName);
                    _mainViewModel.CurrentState.Save();
                }
                else if (extension.EndsWith(".ly"))
                {
                    _musicLoader.SaveToLilypond(saveFileDialog.FileName);
                    _mainViewModel.CurrentState.Save();
                }
                else if (extension.EndsWith(".pdf"))
                {
                    _musicLoader.SaveToPDF(saveFileDialog.FileName);
                    _mainViewModel.CurrentState.Save();
                }
                else
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                }
            }
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
