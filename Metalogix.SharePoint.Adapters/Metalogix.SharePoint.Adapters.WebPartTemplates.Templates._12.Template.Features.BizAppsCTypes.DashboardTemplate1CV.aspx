<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.Portal.WebControls.DashboardPage,Microsoft.SharePoint.Portal,Version=12.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="OSRVWC" Namespace="Microsoft.Office.Server.WebControls" Assembly="Microsoft.Office.Server, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SPSWC" Namespace="Microsoft.SharePoint.Portal.WebControls" Assembly="Microsoft.SharePoint.Portal, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SEARCHWC" Namespace="Microsoft.Office.Server.Search.WebControls" Assembly="Microsoft.Office.Server.Search, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content runat="server" ContentPlaceHolderId="PlaceHolderPageTitle">
    <SPSWC:FormattedListItemProperty id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <SharePoint:ListItemProperty id="PageTitle2" Property="Title" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderLeftNavBarTop" runat="server">
    <div class="ms-quicklaunchouter" Id="filterZoneLeftNavDiv" runat="server">
        <div class="ms-quickLaunch" style="width:100%">
            <table border="0" cellpadding="0" cellspacing="0" ID="FilterZoneTable" width="100%">
                <tr>
                    <td>
                        <table class="ms-navheader" cellpadding="0" cellspacing="0" border="0" width="100%">
                            <tr>
                                <td style="width:100%;font-weight:bold;color:#003399;text-decoration:none">
                                    <asp:Literal runat="server" Text="<%$Resources:sps,FiltersWebPartGroup%>"/>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr ID="FilterRow">
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" width="100%" class="ms-navSubMenu2">
                            <tr>
                                <td valign="top">
                                    <WebPartPages:WebPartZone runat="server" AllowPersonalization="false" ID="FilterZone" Title="<%$Resources:sps,DashboardTemplate_FilterZone%>" Orientation="Vertical" QuickAdd-GroupNames="Filters" QuickAdd-ShowListsAndLibraries="false" QuickAdd-ButtonText="<%$Resources:sps,DashboardTemplate_FilterZoneButtonText%>" BorderWidth="3"/>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" ID="TopZoneTable" width="100%">
        <tr ID="TopRow">
            <td valign="top" ID="TopLeftCell" width="75%">
                <WebPartPages:WebPartZone runat="server" AllowPersonalization="false" ID="TopLeftZone" Title="<%$Resources:sps,DashboardTemplate_TopLeftZone%>" Orientation="Vertical" QuickAdd-GroupNames="Dashboard" BorderWidth="3"/>
            </td>
            <td valign="top" ID="TopRightCell" width="25%">
                <WebPartPages:WebPartZone runat="server" AllowPersonalization="false" ID="TopRightZone" Title="<%$Resources:sps,DashboardTemplate_TopRightZone%>" Orientation="Vertical" QuickAdd-GroupNames="Dashboard" BorderWidth="3"/>
            </td>
        </tr>
    </table>

    <table border="0" cellpadding="0" cellspacing="0" ID="BottomZoneTable" width="100%">
        <tr ID="BottomRow" class="ms-tzbottom">
            <td valign="top" ID="BottomLeftCell">
                <WebPartPages:WebPartZone runat="server" AllowPersonalization="false" ID="BottomZone" Title="<%$Resources:sps,ScorecardTemplate_BottomZone%>" Orientation="Vertical" QuickAdd-GroupNames="Dashboard" BorderWidth="3"/>
            </td>
        </tr>
    </table>

</asp:Content>