<%-- _lcid="1033" _version="12.0.4518" _dal="1" --%>
<%-- _LocalBinding --%>
<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=12.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
    <SharePoint:ListProperty Property="Title" runat="server"/> - <SharePoint:ListItemProperty Property="BaseName" MaxLength=40 runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    <WebPartPages:WebPartZone runat="server" Title="loc:TitleBar" ID="TitleBar" AllowLayoutChange="false" AllowPersonalization="false"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderTitleAreaClass" runat="server">
    <style type="text/css">
	Div.ms-titleareaframe {
	height: 100%;
	}
	.ms-pagetitleareaframe table {
	background: none;
	}
  </style>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <META Name="GENERATOR" Content="Microsoft SharePoint">
    <META Name="ProgId" Content="SharePoint.WebPartPage.Document">
    <META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=utf-8">
    <META Name="CollaborationServer" Content="SharePoint Team Web Site">
    <script>
	var navBarHelpOverrideKey = "wssmain";
	</script>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderSearchArea" runat="server">
    <SharePoint:DelegateControl runat="server"
                                ControlId="SmallSearchInputBox"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderLeftActions" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
    <SharePoint:ProjectProperty Property="Description" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderBodyRightMargin" runat="server">
    <div height=100% class="ms-pagemargin">
        <IMG SRC="/_layouts/images/blank.gif" width=10 height=1 alt="">
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageImage" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderLeftNavBar" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderNavSpacer" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <table cellpadding="4" cellspacing="0" border="0" width="100%">
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" colspan="3" valign="top" width="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Header" ID="Header" FrameType="TitleBarOnly"/>
            </td>
        </tr>
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" rowspan="4" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:LeftColumn" ID="LeftColumn" FrameType="TitleBarOnly"/>
            </td>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row1" FrameType="TitleBarOnly" Orientation="Horizontal"/>
            </td>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" rowspan="4" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:RightColumn" ID="RightColumn" FrameType="TitleBarOnly"/>
            </td>
        </tr>
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Row2" ID="Row2" FrameType="TitleBarOnly" Orientation="Horizontal"/>
            </td>
        </tr>
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Row3" ID="Row3" FrameType="TitleBarOnly" Orientation="Horizontal"/>
            </td>
        </tr>
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" height="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Row4" ID="Row4" FrameType="TitleBarOnly" Orientation="Horizontal"/>
            </td>
        </tr>
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" colspan="3" valign="top" width="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:Footer" ID="Footer" FrameType="TitleBarOnly"/>
            </td>
        </tr>
        <script language="javascript">if(typeof(MSOLayout_MakeInvisibleIfEmpty) == "function") {MSOLayout_MakeInvisibleIfEmpty();}</script>
    </table>
</asp:Content>