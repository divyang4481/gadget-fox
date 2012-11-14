<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GadgetFox.Login" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

	<p style="height: 0px; margin-top: 0px; font-style: normal; font-size: large; top: auto; text-align:center; margin-bottom: 50px;">Account Login</p>
	<asp:Panel ID="loginPanel" runat="server" BorderColor="#999999" style="margin: 0px auto;" Width="600px" HorizontalAlign="Center">
		<div style="width: 250px; margin: 0px auto;">
            <table>
                <tr>
                    <td><asp:Label ID="Label1" runat="server" Text="Login ID" style="margin-right: 10px;"></asp:Label></td>
                    <td><asp:TextBox ID="txtEmailID" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="Label2" runat="server" Text="Password" style="margin-right: 10px;"></asp:Label></td>
                    <td><asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button ID="Button1" runat="server" Text="Login" OnClick="Button1_Click" style="margin-top: 20px"/></td>
                </tr>
            </table>

            <div style="margin: auto;"><asp:Label runat="server" ID="lblLoginError" ForeColor="Red" Width="200px"></asp:Label></div>
            <div style="margin: auto;"><asp:LinkButton ID="Button2" runat="server" ForeColor="Blue" Text="Create an account" PostBackUrl="~/CreateAccount.aspx"/></div>
            <div style="margin: auto;"><asp:LinkButton ID="Button3" runat="server" ForeColor="Blue" Text="Recover password" PostBackUrl="~/UnderConstruction.aspx"/></div>
            <div style="margin: auto;"><asp:LinkButton ID="Button4" runat="server" ForeColor="Blue" Text="Contact support" PostBackUrl="~/UnderConstruction.aspx"/></div>
      	</div>
    </asp:Panel>      
</asp:Content>



