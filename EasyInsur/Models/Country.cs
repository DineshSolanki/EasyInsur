using System;
using EasyInsur.Modules;

namespace EasyInsur.Models
{
    public class Country
    {
        private string _mobileMask;
        private string _exampleNumber;
        public string name { get; set; }
        public string dial_code { get; set; }
        public string code { get; set; }
        public override string ToString() => name;
        private Tuple<string, string>? _mobileDetails; //item1 mask, item 2 number
        public string MobileMask
        {
            get
            {
                if (_mobileMask is not null) return _mobileMask;
                if (_mobileDetails is not null)
                {
                    _mobileMask = _mobileDetails.Item1;
                }
                else
                {
                    _mobileDetails = Util.GetMobileDetails(code);
                    _mobileMask = _mobileDetails.Item1;
                }

                return _mobileMask;
            }
            set => _mobileMask = value;
        }

        public string ExampleNumber
        {
            get
            {
                if (_exampleNumber is not null) return _exampleNumber;
                if (_mobileDetails is not null)
                {
                    _exampleNumber = _mobileDetails.Item2;
                }
                else
                {
                    _mobileDetails = Util.GetMobileDetails(code);
                    _exampleNumber = _mobileDetails.Item2;
                }

                return _exampleNumber;
            }
            set => _exampleNumber = value;
        }
    }
}
