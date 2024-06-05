using Module1.Services;
using OrganDRR_sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace testApp.ViewModels
{
    public class OrgansCheckListViewModel : BindableBase
    {
        private IContourDRRService _contourDrr;
        public OrgansCheckListViewModel(IContourDRRService contourDRRService)
        {
            _contourDrr = contourDRRService;
        }
        public void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                switch (checkBox.Content.ToString())
                {
                    case "Skin":
                        //getOrganDRR(0);
                        _contourDrr.setOrganNumber(0);
                        break;
                    case "R.Lung":
                        //getOrganDRR(1);
                        _contourDrr.setOrganNumber(1);
                        break;
                    case "R.Kidney":
                        //getOrganDRR(2);
                        _contourDrr.setOrganNumber(2);
                        break;
                    case "L.Kidney":
                        //getOrganDRR(3);
                        _contourDrr.setOrganNumber(3);
                        break;
                    case "L.Lung":
                        //getOrganDRR(4);
                        _contourDrr.setOrganNumber(4);
                        break;
                    case "Spinal_Cord":
                        //getOrganDRR(5); 
                        _contourDrr.setOrganNumber(5);
                        break;
                    case "Brain":
                        //getOrganDRR(6);
                        _contourDrr.setOrganNumber(6);
                        break;
                }
            }

        }
    }
}
