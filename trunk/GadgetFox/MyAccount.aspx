<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs" Inherits="GadgetFox.MyAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
    <link href="css/gadgetfox.css" rel="stylesheet" type="text/css" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlIM" runat="server">
        <div id="banner">
            <h3>My Account</h3>
        </div>
        <div id="columns">
            <div id="side">
                <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/EditPersonalInformation.aspx">Edit Personal Information</asp:LinkButton>
                <br />
                <asp:LinkButton ID="btnMonitorInventory" runat="server" PostBackUrl="~/MonitorInventory.aspx">Monitor Inventory</asp:LinkButton>
                <br />
                <asp:LinkButton ID="btnViewInventory" runat="server" PostBackUrl="~/ViewInventory.aspx">View/Edit Inventory</asp:LinkButton>

            </div>
            <div id="main">
                
            </div>
        </div>
    </asp:Panel>
</asp:Content>
