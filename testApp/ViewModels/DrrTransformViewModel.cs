using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Module1;
using Module1.ViewModels;
using Kitware.VTK;

namespace testApp.ViewModels
{
    public class DrrTransformViewModel : BindableBase
    {
        #region Properties
        #region Gant Angel prop
        private int _txtGantAngel = 0;
        public int txtGantAngel
        {
            get { return _txtGantAngel; }
            set
            {
                _txtGantAngel = value;
                SetProperty(ref _txtGantAngel, value);
            }
        }
        #endregion
        #region Table Angle Prop
        private int _txtTableAngle = 0;
        public int txtTableAngle
        {
            get { return _txtTableAngle; }
            set
            {
                _txtTableAngle = value;
                SetProperty(ref _txtTableAngle,value);
            }
        }
        #endregion
        private IDrrGeneratorService _drrGeneratorService;
        private vtkImageData imageData;
        private vtkRenderWindow _renderWindow;

        #endregion
        #region Methods
        #region Constructor
        public DrrTransformViewModel(IDrrGeneratorService drrGeneratorService)
        {

            _drrGeneratorService = drrGeneratorService;
        }
        #endregion
        #region Calculate Drr Button
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            _drrGeneratorService.setTableAngle(txtTableAngle);
            _drrGeneratorService.setGantryAngle(txtGantAngel);
            _drrGeneratorService.updateRenderer();
        }
        #endregion

        #endregion
    }
}
