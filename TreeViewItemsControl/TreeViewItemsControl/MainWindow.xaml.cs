using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TreeViewItemsControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point _lastMouseDown;
        TreeViewItem draggedItem, _target;

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Add Item, Add SubItem, Delete Item SubItem
        private void ButtonNewItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem Child = new TreeViewItem();
            Child.Header = TextBoxNewItem.Text;
            TreeViewControl.Items.Add(Child);
            TextBoxNewItem.Text = "";
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

        #endregion

        #region Drag and Drop Items

        private void TreeViewControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(TreeViewControl);
            }
        }
        private void TreeViewControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(TreeViewControl);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (TreeViewItem)TreeViewControl.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(TreeViewControl, TreeViewControl.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Header.ToString().Equals(_target.Header.ToString()))
                                {
                                    CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void TreeViewControl_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(TreeViewControl);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }
        private void TreeViewControl_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception)
            {
            }
        }
        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }
        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Você gostaria de mover o item: " + _sourceItem.Header.ToString() + " dentro: " + _targetItem.Header.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    addChild(_sourceItem, _targetItem);

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                    // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                    if (ParentItem == null)
                    {
                        TreeViewControl.Items.Remove(_sourceItem);
                    }
                    else
                    {
                        ParentItem.Items.Remove(_sourceItem);
                    }
                }
                catch
                {

                }
            }

        }
        public void addChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            // add item in target TreeViewItem 
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = _sourceItem.Header;
            _targetItem.Items.Add(item1);
            foreach (TreeViewItem item in _sourceItem.Items)
            {
                addChild(item, item1);
            }
        }
        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        #endregion

    }
}
