<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="EditPersonalInformation.aspx.cs" Inherits="GadgetFox.EditPersonalInformation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-top: 0px; font-style: normal; font-size: large; top: auto;">
        Edit Personal Information&nbsp;</p>
    <asp:Panel ID="editPersonalInformationPanel" runat="server" BorderColor="#999999" Height="308px" style="margin-left: 197px" Width="589px">
        <br />
        <br />
        <table>
            <tr>
                <td>First Name<asp:Label ID="Label1" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:TextBox ID="firstNameTB" runat="server" style="margin-left: 0px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="firstNameTB" ForeColor="Red" InitialValue="First Name"></asp:RequiredFieldValidator> 
                </td>
            </tr>
            <tr>
                <td>Last Name<asp:Label ID="Label2" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:TextBox ID="lastNameTB" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="LastNameTB" ForeColor="Red" InitialValue="Last Name"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>Date of Birth<asp:Label ID="Label3" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:TextBox ID="birthDateTB" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="dobCalender" runat="server" TargetControlID="birthDateTB"></ajaxToolkit:CalendarExtender> 
                </td>
            </tr>
            <tr>
                <td>Phone Number<asp:Label ID="Label4" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:TextBox ID="phoneNumberTB" runat="server"></asp:TextBox>
                    <ajaxToolkit:MaskedEditExtender runat="server" ID="mEExtenderPhone" TargetControlID="phoneNumberTB" Mask="\(999\)999\-9999"></ajaxToolkit:MaskedEditExtender> 
                    <ajaxToolkit:MaskedEditValidator runat="server" ID="mEValidatorPhone" ForeColor="Red" ControlToValidate="phoneNumberTB" ValidationExpression="\d{10}" ControlExtender="mEExtenderPhone"></ajaxToolkit:MaskedEditValidator>
                </td>
            </tr>
            <tr>
                <td><asp:Button ID="saveButton" runat="server" Text="Save" Width="75px" OnClick="saveButton_Click" style="margin-top: 20px"/></td>
                <td><asp:Button ID="cancelButton" runat="server" Text="Cancel" Width="75px" PostBackUrl="~/Home.aspx" style="margin-top: 20px"/></td>
            </tr>
        </table>
    </asp:Panel>
    </asp:Content>
