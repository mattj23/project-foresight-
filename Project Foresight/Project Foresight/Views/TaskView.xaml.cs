﻿using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using Project_Foresight.Tools;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Views
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        private Point _mouseDownPoint;
        private bool _isDragging;

        public MouseButtonEventHandler ClickHandler;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TaskViewModel), typeof(TaskView), new PropertyMetadata(default(TaskViewModel)));

        public static readonly DependencyProperty LayoutElementProperty = DependencyProperty.Register(
            "LayoutElement", typeof(IInputElement), typeof(TaskView), new PropertyMetadata(default(IInputElement)));

        public IInputElement LayoutElement
        {
            get { return (IInputElement) GetValue(LayoutElementProperty); }
            set { SetValue(LayoutElementProperty, value); }
        }

        public TaskViewModel ViewModel
        {
            get { return (TaskViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public TaskView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point canvasPoint = e.GetPosition(this.LayoutElement);

                this.ViewModel.X += (canvasPoint.X - _mouseDownPoint.X);
                this.ViewModel.Y += (canvasPoint.Y - _mouseDownPoint.Y);
                this._mouseDownPoint = canvasPoint;
            }

        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDownPoint = e.GetPosition(this.LayoutElement);
                this.BringToFront();
                _isDragging = true;

                this.ViewModel.Parent.SelectedTask = this.ViewModel;
            }
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
                _isDragging = false;
        }

        private void BringToFront()
        {
            var maxZ = this.ViewModel.Parent.Tasks.Select(x => x.ZIndex).Max();
            this.ViewModel.ZIndex = maxZ + 1;
        }

        private void EditBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                (sender as TextBox).IsReadOnly = true;
            }
        }

        private void EditBoxLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).IsReadOnly = true;
        }

        private void EditBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox) sender;
            textBox.IsReadOnly = false;

            DelayedAction.Execute(textBox.SelectAll, 50);
        }
    }
}