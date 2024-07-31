# Simply Postcode Desktop c# Example - Postcode Address Finder
 
This example demonstrates how to use Simply Postcode to implement a Full address searched by
Postcode, Postcode + Part of address or words (search as you type), return a list of matching addresses.

[Open on line account to get API Key](https://www.simplylookupconsole.co.uk/wizards/OpenAccount.aspx?coid=30_30_30_______)

[Click here for more details on the API](https://www.simplypostcode.com/address-finder-open-api#getselectedaddress)

[Swagger API UI](https://api.simplylookupadmin.co.uk)

For easy web integration, we also offer "SimplyCompleteAI Address Finder", which can be added to any site using one line of code. [More info on SimplyCompleteAI](https://www.simplypostcode.com/address-finder-for-web-site)

[Prices](https://www.simplypostcode.com/pricing/price-list)

[What makes us different?](https://www.simplypostcode.com/royal-mail-paf-daily-data)

## Screenshots of Example Program
1. Search by postcode<br>
![Screenshot of it in action.](/images/screen1.png)

2. Search by Words<br>
![Screenshot of it in action.](/images/screen2.png)

## The Code Structure

The main portion of the code is in `cSharpeCode/Form1.cs`, which does all the search work.  
`cSharpeCode/AppCode/Settings.cs` is used to store the API key, in the registry, which is obtained within your Simply Postcode online account.
`cSharpeCode/AppCode/typeDefs.cs` defines a handful of data structures that represent the JSON data returned by the API.

## The basic principle is as follows.   

The following text was used with Chat GPT to create the basic example and could be used to create in different languages/platforms etc.

### Chat GPT Question used to create such an Example

First in c# .Net App we created a form with:
1. textBoxFind  - For user to type address into
2. listBoxAddressLines  - To display the list of results
3. textBoxAddress  - multiline textbox to display the Full Address Details
4. labelInstructions  - Label for instructions set to "Type Postcode, Postcode + Part of address or words to search"

Note also we are calling `full_v3/getaddresslist` with `&options=B` which will Add `<b>` and `</b>` around the results matching text.  You may not want this.

Simply change "In c#, .net 4.8 Windows app, " to what ever ... and ask chat GPT...

```
In c#, .net 4.8 Windows app, I need to add to a text box "textBoxFind" to perform a search, which 
calls an API given at https://api.simplylookupadmin.co.uk.   

I would suggest on key up event, of this textbox, if user stops typing for 300 ms call

"https://api.simplylookupadmin.co.uk/full_v3/getaddresslist?data_api_Key=APIKEY&query=texttosearch&options=B" to 
populate a selection box called "listBoxAddressLines".  

Example. If texttosearch="PE13 2QL", this API call will return json

"{

  "results": [

    {

      "Line": "1 Victoria Road Wisbech <b>PE13 2QL</b>",

      "ID": "11570811_0S"

    },

    {

      "Line": "10 Victoria Road Wisbech <b>PE13 2QL</b>",

      "ID": "11570812_0S"

    },

    {

      "Line": "Victoria Lodge 18 Victoria Road Wisbech <b>PE13 2QL</b>",

      "ID": "31597197_2535693S"

    }

  ],

  "inputid": 0,

  "queryid": 0,

  "processResult": true,

  "instructionsHtml": "Now enter House Name or Number.",
  "instructionsTxt": "Now enter House Name or Number.",
  "finishword": "",

  "errormessage": ""

}" display this for user selection, and then if the user selects a line, call 
full_v3/getselectedaddress with api key and ID of line selected.   And 
update "labelInstructions" label with "instructionsHtml" from the 
json results.

Also, if one line is returned in the results, and the user tabs out of the 
"textBoxFind" text box, we make it select first item as if the user
selected it.
And the in the results line Bold the text marked with HTML "<b>" starts 
the bold and "</b> ends the Bold.

When the user selects a line in the results, the API call 
full_v3/getselectedaddress?data_api_Key=APIKEY&id=Selected line id, 
will return json

"{ 
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
}"

Then, put the returned address fields "organisation,line1,line2,line3,town,county,postcode,
country,found,licenseStatus" into a multiline textbox called "textBoxAddress."  
```

## Other Refinements
We added code to hide the select box when not in use

On the last call "full_v3/getaddresslist"
1. queryid - If you are implementing search as you type, you can supply a counter in this parameter, which is returned. This allows you to process the most recent query results and ignore any 
older responses.
2. inputid - If you have multiple address forms, you can give each a number, which is returned in the response. Then your response code can then act appropriately.

On the last call "full_v3/getselectedaddress"
1. homelocation - if set to a valid postcode, or 'lat,long', it will calculate the distance from the Address
2. inputid - If you have multiple address forms, you can give each a number, which is returned in the response. Your response code can act appropriately.
3. userid - If used within an organisation by employees of a company with a user-based license, then this should identify the call for data used to count the users. User Name or GUID relating to the user. (must be 8 Chars or More).  This is a requirement of the Royal Mail Terms & Conditions. Not required if credit packs are purchased.