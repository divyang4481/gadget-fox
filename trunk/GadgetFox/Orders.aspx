<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="GadgetFox.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
    <p style="height: 0px; margin-bottom: 50px; font-style: normal; font-size: large; top: auto;">
        Orders
    </p>
    <div>
        <div>
            <asp:Label ID="Label1" runat="server">Search (by order or email ID):</asp:Label>
            <asp:TextBox ID="criteriaTB" runat="server" style="padding: 5px;"></asp:TextBox>
            <asp:Button ID="SearchBT" runat="server" Text="Search" OnClick="Search_Click" style="padding: 5px;"/>
        </div>
        <asp:GridView ID="GridView1" runat="server" style="margin-top: 20px;" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>
    </div>
</asp:Content>
