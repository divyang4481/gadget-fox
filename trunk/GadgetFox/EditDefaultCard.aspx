﻿<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="EditDefaultCard.aspx.cs" Inherits="GadgetFox.EditDefaultCard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-bottom: 50px; font-style: normal; font-size: large; top: auto;">
        Edit Default Card
    </p>

    <div style="display: block; margin-bottom: 10px;">
        <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
    <div style="display: block;">
        <asp:Panel ID="Panel1" runat="server" runat="server" BorderColor="#999999" Width="479px" Direction="LeftToRight" HorizontalAlign="Left">
            <table>
                <tr>
                    <td>Type of Card<asp:Label ID="Label6" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="cardTypeDL" runat="server" Style="margin-left: 0px">
                            <asp:ListItem>--Select--</asp:ListItem>
                            <asp:ListItem>American Express</asp:ListItem>
                            <asp:ListItem>Discover</asp:ListItem>
                            <asp:ListItem>Master Card</asp:ListItem>
                            <asp:ListItem>Visa</asp:ListItem>
                        </asp:DropDownList>
                        <asp:CompareValidator ID="cVCardType" runat="server" ErrorMessage="Select card type" Text="*" ForeColor="Red" ControlToValidate="cardTypeDL" ValueToCompare="--Select--" Operator="NotEqual"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>First Name<asp:Label ID="Label9" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="firstNameTB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVFirstName" runat="server" ErrorMessage="Enter first name" Text="*" ForeColor="Red" ControlToValidate="firstNameTB"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td>Last Name<asp:Label ID="Label10" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="lastNameTB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVLastName" runat="server" ErrorMessage="Enter last name" Text="*" ForeColor="Red" ControlToValidate="lastNameTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Card Number<asp:Label ID="Label7" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="cardNumberTB" runat="server"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender runat="server" ID="mEExtenderCardNum" TargetControlID="cardNumberTB" Mask="9999\-9999\-9999\-9999" ClearMaskOnLostFocus="False"></ajaxToolkit:MaskedEditExtender>
                        <ajaxToolkit:MaskedEditValidator runat="server" ID="mEValidatorCardNum" ForeColor="Red" ControlToValidate="cardNumberTB" ControlExtender="mEExtenderCardNum" ValidationExpression="\d{4}\-\d{4}\-\d{4}\-\d{4}|____\-____\-____\-____" InvalidValueMessage="Invalid phone number" InvalidValueBlurredMessage="*"></ajaxToolkit:MaskedEditValidator>
                        <asp:RequiredFieldValidator ID="rFVCardNumber" runat="server" ErrorMessage="Enter card number" Text="*" ForeColor="Red" ControlToValidate="cardNumberTB" InitialValue="____-____-____-____"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>CVV<asp:Label ID="Label11" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="cvvTB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVCVV" runat="server" ErrorMessage="Enter CVV" Text="*" ForeColor="Red" ControlToValidate="cvvTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Expiration Date<asp:Label ID="Label8" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="expMonthTB" runat="server" Style="margin-left: 0px" Width="30px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVExpMonth" runat="server" ErrorMessage="Enter expiration month" Text="*" ForeColor="Red" ControlToValidate="expMonthTB"></asp:RequiredFieldValidator>
                        <asp:TextBox ID="expYearTB" runat="server" Width="60px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVExpYear" runat="server" ErrorMessage="Enter expiration year" Text="*" ForeColor="Red" ControlToValidate="expYearTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Address 1<asp:Label ID="Label1" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="address1TB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVAddress" runat="server" ErrorMessage="Enter address" Text="*" ForeColor="Red" ControlToValidate="expYearTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Address 2</td>
                    <td>
                        <asp:TextBox ID="address2TB" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>City<asp:Label ID="Label2" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="cityTB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rVCity" runat="server" ErrorMessage="Enter city" Text="*" ForeColor="Red" ControlToValidate="cityTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>State<asp:Label ID="Label3" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="stateDL" runat="server" Width="131px">
                            <asp:ListItem>--Select--</asp:ListItem>
                            <asp:ListItem>Alabama</asp:ListItem>
                            <asp:ListItem>Alaska</asp:ListItem>
                            <asp:ListItem>Arizona</asp:ListItem>
                            <asp:ListItem>Arkansas</asp:ListItem>
                            <asp:ListItem>California</asp:ListItem>
                            <asp:ListItem>Colorado</asp:ListItem>
                            <asp:ListItem>Connecticut</asp:ListItem>
                            <asp:ListItem>Delaware</asp:ListItem>
                            <asp:ListItem>Florida</asp:ListItem>
                            <asp:ListItem>Georgia</asp:ListItem>
                            <asp:ListItem>Hawaii</asp:ListItem>
                            <asp:ListItem>Idaho</asp:ListItem>
                            <asp:ListItem>Illinois</asp:ListItem>
                            <asp:ListItem>Indiana</asp:ListItem>
                            <asp:ListItem>Iowa</asp:ListItem>
                            <asp:ListItem>Kansas</asp:ListItem>
                            <asp:ListItem>Kentucky</asp:ListItem>
                            <asp:ListItem>Louisiana</asp:ListItem>
                            <asp:ListItem>Maine</asp:ListItem>
                            <asp:ListItem>Maryland</asp:ListItem>
                            <asp:ListItem>Massachusetts</asp:ListItem>
                            <asp:ListItem>Michigan</asp:ListItem>
                            <asp:ListItem>Minnesota</asp:ListItem>
                            <asp:ListItem>Mississippi</asp:ListItem>
                            <asp:ListItem>Missouri</asp:ListItem>
                            <asp:ListItem>Montana</asp:ListItem>
                            <asp:ListItem>Nebraska</asp:ListItem>
                            <asp:ListItem>Nevada</asp:ListItem>
                            <asp:ListItem>New Hampshire</asp:ListItem>
                            <asp:ListItem>New Jersey</asp:ListItem>
                            <asp:ListItem>New Mexico</asp:ListItem>
                            <asp:ListItem>New York</asp:ListItem>
                            <asp:ListItem>North Carolina</asp:ListItem>
                            <asp:ListItem>North Dakota</asp:ListItem>
                            <asp:ListItem>Ohio</asp:ListItem>
                            <asp:ListItem>Oklahoma</asp:ListItem>
                            <asp:ListItem>Oregon</asp:ListItem>
                            <asp:ListItem>Pennsylvania</asp:ListItem>
                            <asp:ListItem>Rhode Island</asp:ListItem>
                            <asp:ListItem>South Carolina</asp:ListItem>
                            <asp:ListItem>South Dakota</asp:ListItem>
                            <asp:ListItem>Tennessee</asp:ListItem>
                            <asp:ListItem>Texas</asp:ListItem>
                            <asp:ListItem>Utah</asp:ListItem>
                            <asp:ListItem>Vermont</asp:ListItem>
                            <asp:ListItem>Virginia</asp:ListItem>
                            <asp:ListItem>Washington</asp:ListItem>
                            <asp:ListItem>West Virginia</asp:ListItem>
                            <asp:ListItem>Wisconsin</asp:ListItem>
                            <asp:ListItem>Wyoming</asp:ListItem>
                        </asp:DropDownList>
                        <asp:CompareValidator ID="cVState" runat="server" ErrorMessage="Select state" Text="*" ForeColor="Red" ControlToValidate="stateDL" ValueToCompare="--Select--" Operator="NotEqual"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>Country<asp:Label ID="Label4" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="countryDL" runat="server" Width="135px" Enabled="false">
                            <asp:ListItem>United States</asp:ListItem>
                            <asp:ListItem>Argentina</asp:ListItem>
                            <asp:ListItem>Australia</asp:ListItem>
                            <asp:ListItem>Bahamas</asp:ListItem>
                            <asp:ListItem>Bahrain</asp:ListItem>
                            <asp:ListItem>Bangladesh</asp:ListItem>
                            <asp:ListItem>Bermuda</asp:ListItem>
                            <asp:ListItem>Bolivia</asp:ListItem>
                            <asp:ListItem>Brazil</asp:ListItem>
                            <asp:ListItem>British Virgin Islands</asp:ListItem>
                            <asp:ListItem>Brunei</asp:ListItem>
                            <asp:ListItem>Canada</asp:ListItem>
                            <asp:ListItem>Chile</asp:ListItem>
                            <asp:ListItem>China</asp:ListItem>
                            <asp:ListItem>Colombia</asp:ListItem>
                            <asp:ListItem>Costa Rica</asp:ListItem>
                            <asp:ListItem>Croatia</asp:ListItem>
                            <asp:ListItem>Dominicia</asp:ListItem>
                            <asp:ListItem>Dominican Republic</asp:ListItem>
                            <asp:ListItem>Egypt</asp:ListItem>
                            <asp:ListItem>Ethipoia</asp:ListItem>
                            <asp:ListItem>Europa Island</asp:ListItem>
                            <asp:ListItem>Fiji</asp:ListItem>
                            <asp:ListItem>Finland</asp:ListItem>
                            <asp:ListItem>France</asp:ListItem>
                            <asp:ListItem>Gabon</asp:ListItem>
                            <asp:ListItem>Georgia</asp:ListItem>
                            <asp:ListItem>Germany</asp:ListItem>
                            <asp:ListItem>Ghana</asp:ListItem>
                            <asp:ListItem>Haiti</asp:ListItem>
                            <asp:ListItem>Honduras</asp:ListItem>
                            <asp:ListItem>Hong Kong</asp:ListItem>
                            <asp:ListItem>Hungary</asp:ListItem>
                            <asp:ListItem>Iceland</asp:ListItem>
                            <asp:ListItem>India</asp:ListItem>
                            <asp:ListItem>Indonesia</asp:ListItem>
                            <asp:ListItem>Iran</asp:ListItem>
                            <asp:ListItem>Iraq</asp:ListItem>
                            <asp:ListItem>Ireland</asp:ListItem>
                            <asp:ListItem>Israel</asp:ListItem>
                            <asp:ListItem>Italy</asp:ListItem>
                            <asp:ListItem>Jamaica</asp:ListItem>
                            <asp:ListItem>Japan</asp:ListItem>
                            <asp:ListItem>Jordan</asp:ListItem>
                            <asp:ListItem>KazaKhstan</asp:ListItem>
                            <asp:ListItem>Kenya</asp:ListItem>
                            <asp:ListItem>Kuwait</asp:ListItem>
                            <asp:ListItem>Liberia</asp:ListItem>
                            <asp:ListItem>Libya</asp:ListItem>
                            <asp:ListItem>Lithuania</asp:ListItem>
                            <asp:ListItem>Macau</asp:ListItem>
                            <asp:ListItem>Madagascar</asp:ListItem>
                            <asp:ListItem>Malaysia</asp:ListItem>
                            <asp:ListItem>Maldives</asp:ListItem>
                            <asp:ListItem>Mauritius</asp:ListItem>
                            <asp:ListItem>Mexico</asp:ListItem>
                            <asp:ListItem>Monaco</asp:ListItem>
                            <asp:ListItem>Mongolia</asp:ListItem>
                            <asp:ListItem>Morocco</asp:ListItem>
                            <asp:ListItem>Namibia</asp:ListItem>
                            <asp:ListItem>Nepal</asp:ListItem>
                            <asp:ListItem>Netherlands</asp:ListItem>
                            <asp:ListItem>New Zealand</asp:ListItem>
                            <asp:ListItem>Nicaragua</asp:ListItem>
                            <asp:ListItem>Nigeria</asp:ListItem>
                            <asp:ListItem>Norway</asp:ListItem>
                            <asp:ListItem>Oman</asp:ListItem>
                            <asp:ListItem>Pakistan</asp:ListItem>
                            <asp:ListItem>Panama</asp:ListItem>
                            <asp:ListItem>Papua New Guinea</asp:ListItem>
                            <asp:ListItem>Paraguay</asp:ListItem>
                            <asp:ListItem>Peru</asp:ListItem>
                            <asp:ListItem>Philippines</asp:ListItem>
                            <asp:ListItem>Poland</asp:ListItem>
                            <asp:ListItem>Portugal</asp:ListItem>
                            <asp:ListItem>Puerto Rica</asp:ListItem>
                            <asp:ListItem>Qatar</asp:ListItem>
                            <asp:ListItem>Romania</asp:ListItem>
                            <asp:ListItem>Russia</asp:ListItem>
                            <asp:ListItem>Samoa</asp:ListItem>
                            <asp:ListItem>Saudi Arabia</asp:ListItem>
                            <asp:ListItem>Singapore</asp:ListItem>
                            <asp:ListItem>South Africa</asp:ListItem>
                            <asp:ListItem>Spain</asp:ListItem>
                            <asp:ListItem>Srilanka</asp:ListItem>
                            <asp:ListItem>Sudan</asp:ListItem>
                            <asp:ListItem>Swedan</asp:ListItem>
                            <asp:ListItem>Switzerland</asp:ListItem>
                            <asp:ListItem>Taiwan</asp:ListItem>
                            <asp:ListItem>Tajikistan</asp:ListItem>
                            <asp:ListItem>Tanazania</asp:ListItem>
                            <asp:ListItem>Thailand</asp:ListItem>
                            <asp:ListItem>Turkey</asp:ListItem>
                            <asp:ListItem>Uganda</asp:ListItem>
                            <asp:ListItem>Ukraine</asp:ListItem>
                            <asp:ListItem>United Arab Emirates</asp:ListItem>
                            <asp:ListItem>United Kingdom</asp:ListItem>
                            <asp:ListItem>United States</asp:ListItem>
                            <asp:ListItem>Uruguay</asp:ListItem>
                            <asp:ListItem>Uzbekistan</asp:ListItem>
                            <asp:ListItem>Venezuela</asp:ListItem>
                            <asp:ListItem>Vietnam</asp:ListItem>
                            <asp:ListItem>Virgin Islands</asp:ListItem>
                            <asp:ListItem>Yemen</asp:ListItem>
                            <asp:ListItem>Zambia</asp:ListItem>
                            <asp:ListItem>Zimbabwe</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Zip code<asp:Label ID="Label5" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="zipcodeTB" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rFVZip" runat="server" ErrorMessage="Enter zip code" Text="*" ForeColor="Red" ControlToValidate="zipcodeTB"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="saveButton" runat="server" Text="Submit" Width="75px" OnClick="saveButton_Clicked" Style="margin-top: 20px" />
                    </td>
                    <td>
                        <asp:Button ID="cancelButton" runat="server" Text="Cancel" Width="75px" PostBackUrl="~/MyAccount.aspx" Style="margin-top: 20px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"><br/></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:ValidationSummary ID="validationSummary" runat="server" HeaderText="Not able to save default card. Please correct the following errors and try again." />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>

