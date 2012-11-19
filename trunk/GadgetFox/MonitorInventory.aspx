<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="MonitorInventory.aspx.cs" Inherits="GadgetFox.MonitorInventory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="gridViewWidth">
        <asp:GridView ID="gdvMonitorInventory" runat="server" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None"
            BorderWidth="1px" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gdvMonitorInventory_Sorting">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="ProductID" SortExpression="ProductID">
                    <ItemTemplate>
                        <%# Eval("ProductID") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                    <ItemTemplate>
                        <%# Eval("Name") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Description">
                    <ItemTemplate>
                        <%# Eval("Description") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Price" SortExpression="Price">
                    <ItemTemplate>
                        <%# Eval("Price") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SalePrice" SortExpression="SalePrice">
                    <ItemTemplate>
                        <%# Eval("SalePrice") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="InSale">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Enabled="false" Checked='<%# Eval("InSale") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                    <ItemTemplate>
                        <%# Eval("Quantity") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CategoryID" SortExpression="CategoryID">
                    <ItemTemplate>
                        <%# Eval("CategoryID") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SubCategoryID" SortExpression="SubCategoryID">
                    <ItemTemplate>
                        <%# Eval("SubCategoryID") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Image">
                    <ItemTemplate>
                        <%# Eval("Quantity") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Color" SortExpression="Color">
                    <ItemTemplate>
                        <%# Eval("Color") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Weight">
                    <ItemTemplate>
                        <%# Eval("Weight") %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNoData" runat="server" Text="Inventory is empty."></asp:Label>
            </EmptyDataTemplate>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
    </div>
</asp:Content>
