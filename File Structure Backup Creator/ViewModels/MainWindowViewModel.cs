using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using File_Structure_Backup_Creator.Models;
using File_Structure_Backup_Creator.Views;
using Ookii.Dialogs.Wpf;

namespace File_Structure_Backup_Creator.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowModel MainWindowModel { get; set; }
        public ICommand cmdIniciar { get; set; }
        public ICommand cmdPastaInput { get; set; }
        public ICommand cmdPastaOutput { get; set; }
        public MainWindowViewModel()
        {
            MainWindowModel = new MainWindowModel();
            cmdIniciar = new Commands.CommandIniciar();
            cmdPastaInput = new Commands.CommandPastaInput();
            cmdPastaOutput = new Commands.CommandPastaOutput();
        }
    }

    public class Commands
    {
        public class CommandIniciar : ICommand
        {
            public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

            public bool CanExecute(object parameter)
            {
                return parameter is MainWindowViewModel && (parameter as MainWindowViewModel).MainWindowModel.ValidarObjeto();
            }

            public void Execute(object parameter)
            {
                MainWindowViewModel MainWindowVM = parameter as MainWindowViewModel;
                string sPastaOrigem = MainWindowVM.MainWindowModel.sInput;
                string sPastaDestino = MainWindowVM.MainWindowModel.sOutput;

                try
                {
                    if (Directory.Exists(sPastaOrigem))
                    {
                        DirectoryInfo oPastaOrigem = new DirectoryInfo(sPastaOrigem);
                        foreach (var item in oPastaOrigem.EnumerateDirectories("*", SearchOption.AllDirectories))
                        {
                            var pastaTemp = Path.Combine(sPastaDestino, item.FullName.Remove(0, sPastaOrigem.Length + 1));
                            if (!Directory.Exists(pastaTemp))
                                Directory.CreateDirectory(pastaTemp);
                        }
                        foreach (var item in oPastaOrigem.EnumerateFiles("*", SearchOption.AllDirectories))
                        {
                            var arquivoTemp = Path.Combine(sPastaDestino, item.FullName.Remove(0, sPastaOrigem.Length + 1));
                            if (!File.Exists(arquivoTemp))
                                File.Create(arquivoTemp);
                        }
                    }

                    MessageBox.Show("Operação efetuada com sucesso.", "File Structure Backup Creator", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + "Operação não concluída.", "File Structure Backup Creator", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class CommandPastaInput : ICommand
        {
            public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

            public bool CanExecute(object parameter)
            {
                return parameter is MainWindowViewModel;
            }

            public void Execute(object parameter)
            {
                MainWindowViewModel MainWindowVM = parameter as MainWindowViewModel;
                VistaFolderBrowserDialog fileDialog = new VistaFolderBrowserDialog() { Description = "Selecione uma pasta para copiar.", SelectedPath = MainWindowVM.MainWindowModel.sInput, ShowNewFolderButton = true, UseDescriptionForTitle = true };
                if (fileDialog.ShowDialog() == true)
                {
                    MainWindowVM.MainWindowModel.sInput = fileDialog.SelectedPath;
                }
            }
        }

        public class CommandPastaOutput : ICommand
        {
            public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

            public bool CanExecute(object parameter)
            {
                return parameter is MainWindowViewModel;
            }

            public void Execute(object parameter)
            {
                MainWindowViewModel MainWindowVM = parameter as MainWindowViewModel;
                VistaFolderBrowserDialog fileDialog = new VistaFolderBrowserDialog() { Description = "Selecione a pasta para colocar o backup.", SelectedPath = MainWindowVM.MainWindowModel.sOutput, ShowNewFolderButton = true, UseDescriptionForTitle = true };
                if (fileDialog.ShowDialog() == true)
                {
                    MainWindowVM.MainWindowModel.sOutput = fileDialog.SelectedPath;
                }
            }
        }
    }
}
