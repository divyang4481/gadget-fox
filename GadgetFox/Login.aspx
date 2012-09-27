<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite1.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GadgetFox.Login" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

    <asp:Table ID="Table1" runat="server" HorizontalAlign="Center" Width="323px">
        <asp:TableHeaderRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:Label ID="lblLogin" runat="server" Text="Account Login"></asp:Label>
            </asp:TableCell>
        </asp:TableHeaderRow>
        <asp:TableRow runat="server">

            <asp:TableCell runat="server">
                <asp:Label ID="Label1" runat="server" Text="Login ID"></asp:Label>
            </asp:TableCell>



            <asp:TableCell>
                <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
            </asp:TableCell>

        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Center">
                <asp:TextBox ID="txtEmailID" runat="server" Text="UserName"></asp:TextBox>

            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:TextBox ID="txtPassword" runat="server" Text="Password" TextMode="Password"></asp:TextBox>

            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:Button ID="Button1" runat="server" Text="Login" OnClick="Button1_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:Label runat="server" ID="lblLoginError" ForeColor="Red"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:LinkButton ID="Button2" runat="server" Text="Create an Account" PostBackUrl="~/CreateAccount.aspx" />
            </asp:TableCell></asp:TableRow><asp:TableRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:LinkButton ID="Button3" runat="server" Text="Recover Password" />
            </asp:TableCell></asp:TableRow><asp:TableRow>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                <asp:LinkButton ID="Button4" runat="server" Text="Contact Support" />
            </asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>



