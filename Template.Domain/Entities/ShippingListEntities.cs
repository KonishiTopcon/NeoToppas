using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoToppas.Domain.Entities
{
    public class ShippingListEntities : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _unitCode;
        public string UnitCode
        {
            get => _unitCode;
            set
            {
                if (_unitCode != value)
                {
                    _unitCode = value;
                    OnPropertyChanged(nameof(UnitCode));
                }
            }
        }
        private string _unitName;
        public string UnitName
        {
            get => _unitName;
            set
            {
                if (_unitName != value)
                {
                    _unitName = value;
                    OnPropertyChanged(nameof(UnitName));
                }
            }
        }
        private int _shippingCnt;
        public int ShippingCnt
        {
            get => _shippingCnt;
            set
            {
                if (_shippingCnt != value)
                {
                    _shippingCnt = value;
                    OnPropertyChanged(nameof(ShippingCnt));
                }
            }
        }

        private string _bikou;
        public string Bikou
        {
            get => _bikou;
            set
            {
                if (_bikou != value)
                {
                    _bikou = value;
                    OnPropertyChanged(nameof(Bikou));
                }
            }
        }
        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                if (_page != value)
                {
                    _page = value;
                    OnPropertyChanged(nameof(Page));
                }
            }
        }
        private int _registeredCnt;
        public int RegisteredCnt
        {
            get => _registeredCnt;
            set
            {
                if (_registeredCnt != value)
                {
                    _registeredCnt = value;
                    OnPropertyChanged(nameof(RegisteredCnt));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
