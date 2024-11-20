﻿//// RelayCommand.cs
//using System;
//using System.Windows.Input;

//namespace Spotify.Helpers;

//public class RelayCommand : ICommand
//{
//    private readonly Action<object> _execute;
//    private readonly Predicate<object> _canExecute;

//    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
//    {
//        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//        _canExecute = canExecute;
//    }

//    public event EventHandler CanExecuteChanged;

//    protected virtual void OnCanExecuteChanged()
//    {
//        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
//    }

//    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

//    public void Execute(object parameter) => _execute(parameter);
//}
