using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyInsur.Models;

namespace EasyInsur.ViewModels
{
    public class ViewPeopleWindowViewModel : BindableBase
    {
        public ViewPeopleWindowViewModel()
        {

        }

        #region Properties

        private PersonType _personType = PersonType.Agent;
        public PersonType PersonType
        {
            get => _personType;
            set => SetProperty(ref _personType, value);
        }

        #endregion
    }
}
