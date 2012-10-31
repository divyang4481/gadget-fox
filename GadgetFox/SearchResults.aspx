<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="SearchResults.aspx.cs" Inherits="GadgetFox.SearchResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
        Search Result for:
        <asp:Label ID="search_string" runat="server"></asp:Label>
                <asp:GridView ID="GridView1" runat="server"/>
    </p>
</asp:Content>
