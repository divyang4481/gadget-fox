<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite1.Master" AutoEventWireup="true" CodeBehind="CreateAccount.aspx.cs" Inherits="GadgetFox.CreateAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Table ID="Table1" runat="server" HorizontalAlign="Center"  Width="323px">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label1" runat="server" Text="Login ID"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvEmailID" runat="server" ErrorMessage="Enter email id" Text="*" ControlToValidate="txtEmailID" ForeColor="Red" InitialValue="Email Address"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="rgvEmailID" runat="server" ErrorMessage="Enter a valid email id" Text="*" ControlToValidate="txtEmailID" ForeColor="Red" ValidationExpression="\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b"></asp:RegularExpressionValidator>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Enter password" Text="*" ControlToValidate="txtPassword" ForeColor="Red"></asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:TextBox ID="txtEmailID" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label3" runat="server" Text="First Name"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ErrorMessage="Enter first name" Text="*" ControlToValidate="txtFirstName" ForeColor="Red" InitialValue="First Name"></asp:RequiredFieldValidator>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="Label4" runat="server" Text="Last Name"></asp:Label>
               <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ErrorMessage="Enter last name" Text="*" ControlToValidate="txtLastName" ForeColor="Red" InitialValue="Last Name"></asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label5" runat="server" Text="Date of Birth"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="Label6" runat="server" Text="Phone Number"></asp:Label>
                <ajaxToolkit:MaskedEditValidator runat="server" ID="mEValidatorPhone" ForeColor="Red" ControlToValidate="txtPhoneNum" ValidationExpression="\d{10}" InvalidValueMessage="Enter a valid Phone Number" ControlExtender="mEExtenderPhone" InvalidValueBlurredMessage="*"></ajaxToolkit:MaskedEditValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:TextBox ID="txtDOB" runat="server" Text="MM/DD/YYYY"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="dobCalender" runat="server" TargetControlID="txtDOB" ClearTime="True"></ajaxToolkit:CalendarExtender>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtPhoneNum" runat="server" ></asp:TextBox>
                <ajaxToolkit:MaskedEditExtender runat="server" ID="mEExtenderPhone" TargetControlID="txtPhoneNum" Mask="\(999\)999\-9999"></ajaxToolkit:MaskedEditExtender>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click"/>
            </asp:TableCell>
        </asp:TableRow>
                <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
               <asp:ValidationSummary ID="validationSummary" runat="server" HeaderText="Please correct the following errors to create account" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Content>
