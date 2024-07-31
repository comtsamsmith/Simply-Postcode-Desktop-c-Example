using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopExample.AppCode;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DesktopExample
{
    public partial class Form1 : Form
    {
        //Used to store the API key in registery, and base url etc
        Settings settings = new Settings();

        //API Documentation: https://www.simplypostcode.com/address-finder-open-api
        //Swagger UI: https://api.simplylookupadmin.co.uk/index.html

        //Setup a timer to perform a sear, using the API, when user has stopped typing for 300ms
        private System.Timers.Timer typingTimer;
        private const int TypingDelay = 300; // 300 ms delay

        public Form1()
        {
            InitializeComponent();

            //Load saved settings
            if (settings.Load())
                this.textBoxKey.Text = settings.apiKey;

            //Setup a timer to perform a sear, using the API, when user has stopped typing for 300ms
            typingTimer = new System.Timers.Timer { Interval = TypingDelay, AutoReset = false };
            typingTimer.Elapsed += TypingTimer_Elapsed;

            //Create Key up event for searching
            textBoxFind.KeyUp += TextBoxFind_KeyUp;
            listBoxAddressLines.SelectedIndexChanged += ListBoxAddressLines_SelectedIndexChanged;

            //Move search list on top of address box
            listBoxAddressLines.Top=textBoxAddress.Top;
            listBoxAddressLines.Left = textBoxAddress.Left;
            listBoxAddressLines.Size = textBoxAddress.Size;

            //Hide Search list
            ShowList(false);

            //Optional nice features:

            //Bold matching Text in list, function ListBoxAddressLines_DrawItem will bold and text in list maked <b> and </b>
            //and getaddresslist API is called with &options=B to activate bold markers
            listBoxAddressLines.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBoxAddressLines.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxAddressLines_DrawItem);
            settings.options = "B";  //activate bold markers for retsults with matching text

            // Handle the Leave event to select the first item if only one is present
            textBoxFind.Leave += TextBoxFind_Leave;
        }

        private void TextBoxFind_KeyUp(object sender, KeyEventArgs e)
        {
            //Reset timer on every key stroke
            typingTimer.Stop();
            typingTimer.Start();
        }

        private async void TypingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Invoke a seach after 300 ms after user stopped typeing
            string query = (string)textBoxFind.Invoke((Func<string>)(() => textBoxFind.Text));
            if (!string.IsNullOrWhiteSpace(query))
            {
                await GetAddressList(query);
            }
        }

        private async Task GetAddressList(string query)
        {
            //Call for address list of matching results
            //see https://www.simplypostcode.com/address-finder-open-api#getaddresslist

            //&options=B will add bold to matching text "Address <b>matching Text</b> text line"
            string url = $"{Settings.ApiBaseUrl}/full_v3/getaddresslist?query={Uri.EscapeDataString(query)}&data_api_Key={settings.apiKey}&options={settings.options}";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var addressList = Newtonsoft.Json.JsonConvert.DeserializeObject<TypeDefs.AddressListResponse>(jsonResponse);
                Invoke(new Action(() => PopulateListBox(addressList)));      //Put on UI thread to populate list     
            }
            else
            {
                MessageBox.Show("Error retrieving address list.");
            }
        }

        private void TextBoxFind_Leave(object sender, EventArgs e)
        {
            //If user TABS out of search box, and we have 1 resuklt then select it
            if (listBoxAddressLines.Items.Count == 1)
            {
                listBoxAddressLines.SelectedIndex = 0;
                ListBoxAddressLines_SelectedIndexChanged(listBoxAddressLines, EventArgs.Empty);
            }
        }

        private void PopulateListBox(TypeDefs.AddressListResponse resp) //List<TypeDefs.AddressResult> addresses,string instructions)
        {
            //Show Search list
            ShowList(true);

            //Populate address list with results
            listBoxAddressLines.Items.Clear();
            foreach (var address in resp.Results)
            {
                listBoxAddressLines.Items.Add(new TypeDefs.ListBoxItem { DisplayText = address.Line, Id = address.ID });
            }

            //Populate with instructions
            if (listBoxAddressLines.Items.Count == 1)
                labelInstructions.Text = "Press Tab to use this address";
            else
                labelInstructions.Text = resp.instructionsTxt;

        }

        private async void ListBoxAddressLines_SelectedIndexChanged(object sender, EventArgs e)
        {
            //User has selected an address, so get address details
            //You would probably do this on double click. 
            if (listBoxAddressLines.SelectedItem is TypeDefs.ListBoxItem selectedItem)
            {
                ShowList(false);
                await GetSelectedAddress(selectedItem.Id);
            }
        }

        private async Task GetSelectedAddress(string id)
        {
            //Call API to get selected address details
            //If you're on a credits-based license, this call will cost one credit, so you should make sure the user only selects the address
            //they need, not as they scroll through the list.
            //see https://www.simplypostcode.com/address-finder-open-api#getselectedaddress
            string url = $"{Settings.ApiBaseUrl}/full_v3/getselectedaddress?id={Uri.EscapeDataString(id)}&data_api_Key={settings.apiKey}";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var selectedAddress = Newtonsoft.Json.JsonConvert.DeserializeObject<TypeDefs.SelectedAddressResponse>(jsonResponse);

                //Put address in multiline address field
                textBoxAddress.Text = $"{selectedAddress.Organisation}\r\n{selectedAddress.Line1}\r\n{selectedAddress.Line2}\r\n{selectedAddress.Line3}\r\n{selectedAddress.Town}\r\n{selectedAddress.County}\r\n{selectedAddress.Postcode}\r\n{selectedAddress.Country}\r\n\r\n{selectedAddress.found}\r\n{selectedAddress.licenseStatus}";
            }
            else
            {
                MessageBox.Show("Error retrieving selected address.");
            }
        }

        private void ListBoxAddressLines_DrawItem(object sender, DrawItemEventArgs e)
        {
            //in the results line Bold the text marked with HTML "<b>" starts the bold and "</b>" ends the Bold.
            //some platforms it may not be possible to bold items in a list.
            if (e.Index < 0) return;

            var item = listBoxAddressLines.Items[e.Index].ToString();

            e.DrawBackground();
            Brush normalBrush = Brushes.Black;
            Font normalFont = e.Font;
            Font boldFont = new Font(e.Font, FontStyle.Bold);

            float x = e.Bounds.Left;
            int lastIndex = 0;

            while (true)
            {
                int startIndex = item.IndexOf("<b>", lastIndex, StringComparison.OrdinalIgnoreCase);
                int endIndex = item.IndexOf("</b>", lastIndex, StringComparison.OrdinalIgnoreCase);

                if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
                {
                    // Draw remaining normal text if no more tags are found
                    string remainingText = item.Substring(lastIndex);
                    e.Graphics.DrawString(remainingText, normalFont, normalBrush, x, e.Bounds.Top);
                    break;
                }

                // Draw normal text before the bold segment
                string normalText = item.Substring(lastIndex, startIndex - lastIndex);
                if (!string.IsNullOrEmpty(normalText))
                {
                    e.Graphics.DrawString(normalText, normalFont, normalBrush, x, e.Bounds.Top);
                    x += e.Graphics.MeasureString(normalText, normalFont).Width;
                }

                // Draw bold text segment
                string boldText = item.Substring(startIndex + 3, endIndex - (startIndex + 3));
                if (!string.IsNullOrEmpty(boldText))
                {
                    e.Graphics.DrawString(boldText, boldFont, normalBrush, x, e.Bounds.Top);
                    x += e.Graphics.MeasureString(boldText, boldFont).Width;
                }

                lastIndex = endIndex + 4; // Move past the </b> tag
            }

            e.DrawFocusRectangle();
        }




        private void SaveButton_Click(object sender, EventArgs e)
        {
            //Save API key to settings
            settings.apiKey = this.textBoxKey.Text;

            if (settings.Save())
            {
                MessageBox.Show("Saved API Key");
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            // Assuming you have a TextBox control named textBox1
            string textToCopy = textBoxAddress.Text;

            // Copying text to clipboard
            Clipboard.SetText(textToCopy);
        }

        private void ShowList(bool showListState)
        {
            //Show/Hide UI elements
            listBoxAddressLines.Visible = showListState;
            textBoxAddress.Visible = !showListState;
            buttonCopy.Visible = !showListState;
            labelInstructions.Visible = showListState;

            if (showListState)
            {
                label3.Text = "Found:";
            }
            else
            {
                label3.Text = "Address:";
            }
        }


    }
}

