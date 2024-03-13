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
        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Gant Angel prop
        private int _txtGantAngel = 0;
        public int txtGantAngel
        {
            get { return _txtGantAngel; }
            set
            {
                _txtGantAngel = value;
                onPropertyChanged(nameof(txtGantAngel));
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
                onPropertyChanged(nameof(txtTableAngle));
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
            var Drr = _drrGeneratorService.updateImgeData(txtGantAngel, txtTableAngle);
            _drrGeneratorService.updateRenderer();
        }
        #endregion

        #endregion
    }
}
