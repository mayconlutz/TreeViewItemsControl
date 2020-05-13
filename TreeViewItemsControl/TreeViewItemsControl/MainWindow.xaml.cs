using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeViewItemsControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonNewItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem Child = new TreeViewItem();
            Child.Header = TextBoxNewItem.Text;
            TreeViewControl.Items.Add(Child);


        }

        private void ButtonNewSubItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                TreeViewItem item = (TreeViewItem)TreeViewControl.SelectedItem;

                if (TreeViewControl.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um Item para criar um Subitem", "Seleção Item", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult MsgResult = MessageBox.Show("Confirma a criação do subitem: " + TextBoxNewItem.Text.ToUpper() + " na raiz: " + item.Header.ToString().ToUpper() + "?", "Confirma Criar SubItem", MessageBoxButton.YesNo, MessageBoxImage.Information);


                    if (MsgResult == MessageBoxResult.Yes)
                    {
                        TreeViewItem Child = new TreeViewItem();
                        Child.Header = TextBoxNewItem.Text;
                        item.Items.Add(Child);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível criar um Subitem, verifique o ERRO abaixo:\n\n" + ex.ToString(), "Erro ao Criar Subitem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonDeleteSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (TreeViewControl.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um Item para apagá-lo", "Seleção Item", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult MsgResult = MessageBox.Show("Confirma apagar o item:" + (TreeViewControl.SelectedItem as TreeViewItem).Header.ToString().ToUpper(), "Confirma Apagar Item", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (MsgResult == MessageBoxResult.Yes)
                    {
                        
                        TreeViewItem parent = (TreeViewControl.SelectedItem as TreeViewItem).Parent as TreeViewItem; //Pega o pai do item selecionado.

                        if (parent != null) //Verifica se esse item tem um pai
                        {
                            parent.Items.Remove(TreeViewControl.SelectedItem); //Apaga o nó desejado a partir do pai.
                        }
                        else
                        {
                            TreeViewControl.Items.Remove(TreeViewControl.SelectedItem); //Apaga o nó desejado
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível apagar o item, verifique o ERRO abaixo:\n\n" + ex.ToString(), "Erro ao Apagar Item", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
