<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite1.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GadgetFox.Login" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

	<p style="height: 0px; margin-top: 0px; font-style: normal; font-size: x-large; top: auto; text-align:center;">Account Login</p>
	<asp:Panel ID="loginPanel" runat="server" BorderColor="#999999" style="margin: 0px auto;" Width="600px" HorizontalAlign="Center">
		<div style="width: 250px; margin: 0px auto;">
            <br />
			<br />
			<div style="float:left;"><asp:Label ID="Label1" runat="server" Text="Login ID"></asp:Label></div>             
			<div style="float: right;"><asp:TextBox ID="txtEmailID" runat="server"></asp:TextBox></div>
			<br />
			<br />
			<div style="float:left;"><asp:Label ID="Label2" runat="server" Text="Password"></asp:Label></div>
			<div style="float: right;"><asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox></div>
			<br />
			<br /> 
            <asp:Button ID="Button1" runat="server" Text="Login" OnClick="Button1_Click" />
			<br />
			<br />
			<br /> 
            <asp:Label runat="server" ID="lblLoginError" ForeColor="Red"></asp:Label>
			<br /> 
			<br />
            <asp:LinkButton ID="Button2" runat="server" Text="Create an Account" PostBackUrl="~/CreateAccount.aspx" />
			<br /> 
			<br />
            <asp:LinkButton ID="Button3" runat="server" Text="Recover Password" />
 			<br /> 
			<br />
            <asp:LinkButton ID="Button4" runat="server" Text="Contact Support" />
			<br />
			<br /> 
			<br />
      	</div>
    </asp:Panel>      
</asp:Content>



