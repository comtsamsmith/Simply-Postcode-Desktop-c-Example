# Simply Postcode Desktop c# Example
 
This example demonstrates how to use Simply Postcode to implement a Full address searched by
Postcode, Postcode + Part of address or words (search as you type), return a list of matching addresses.

For more details on the API and how to get an API key see https://www.simplypostcode.com/address-finder-open-api#getselectedaddress

For easy web integration, we also offer "SimplyCompleteAI Address Finder" https://www.simplypostcode.com/address-finder-for-web-site , which can be added to any site using online code.

Search by Postcode
![Screenshot of it in action.](/images/screen1.png)

Search by Words
![Screenshot of it in action.](/images/screen2.png)

## The basic principle is as follows.   

The following text was used with Chat GPT to create the basic example and could be used to create in different languages/platforms etc.

### Chat GPT Question 

```
In c#, .net 4.8 Windows app, I need to add to a text box "textBoxFind" to perform a search, which calls an API given at https://api.simplylookupadmin.co.uk.   

I would suggest on key up event, of this textbox, if stopped typing for 300 ms call

"https://api.simplylookupadmin.co.uk/full_v3/getaddresslist?data_api_Key=APIKEY&query=texttosearch" to populate a selection box called "listBoxAddressLines".  This will return json

"{

  "results": [

    {

      "Line": "1 Victoria Road Wisbech PE13 2QL",

      "ID": "11570811_0S"

    },

    {

      "Line": "10 Victoria Road Wisbech PE13 2QL",

      "ID": "11570812_0S"

    },

    {

      "Line": "Victoria Lodge 18 Victoria Road Wisbech PE13 2QL",

      "ID": "31597197_2535693S"

    }

  ],

  "inputid": 0,

  "queryid": 0,

  "processResult": true,

  "errormessage": ""

}" display this for user selection, and then if the user selects a line, call full_v3/getselectedaddress with api key and ID of line selected.

the API call full_v3/getselectedaddress   will return json

{ 
    "found": true, 
    "error": false, 
    "inputid": 0, 
    "licenseStatus": "DEMO Full Address Data for ZZ until ('08 Dec 2024')", 
    "addressID": "", 
    "organisation": "", 
    "line1": "The Limes", 
    "line2": "27 Victoria Road", 
    "line3": "", 
    "town": "Wisbech", 
    "county": "Cambridgeshire", 
    "postcode": "PE13 2QL", 
    "rawpostcode": "PE132QL", 
    "country": "England", 
    "deliverypointsuffix": "1T", 
    "nohouseholds": "1", 
    "smallorg": "N", 
    "pobox": "", 
    "mailsortcode": "51144", 
    "unique1": "The Limes", 
    "unique2": "27", 
    "propertyNo": "27", 
    "propertyName": "The Limes", 
    "streetName": "Victoria Road", 
    "udprn": "18616525", 
    "userid": "", 
    "geoposition": "52.659857,0.1617", 
    "geolongitude": 0.1617, 
    "geolatitude": 52.659857, 
    "geodistanceinkm": 0, 
    "geodistanceinmiles": 0 
}

Then, put the returned address fields "organisation,line1,line2,line3,town,county,postcode,country,found,licenseStatus" into a multiline textbox called "textBoxAddress."  
```



