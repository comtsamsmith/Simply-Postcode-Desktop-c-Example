using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopExample.AppCode
{
    internal class TypeDefs
    {

        //see https://www.simplypostcode.com/address-finder-open-api#getaddresslist
        public class AddressListResponse
        {
            public List<AddressResult> Results { get; set; }

            public string instructionsHtml { get; set; }
            public string instructionsTxt { get; set; }
            public string finishword { get; set; }
        }

        public class AddressResult
        {
            public string Line { get; set; }
            public string ID { get; set; }
        }

        //see https://www.simplypostcode.com/address-finder-open-api#getselectedaddress
        public class SelectedAddressResponse
        {
            public string Organisation { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }

            public bool found { get; set; }
            public string licenseStatus { get; set; }



        }

        public class ListBoxItem
        {
            public string DisplayText { get; set; }
            public string Id { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }
    }
}
