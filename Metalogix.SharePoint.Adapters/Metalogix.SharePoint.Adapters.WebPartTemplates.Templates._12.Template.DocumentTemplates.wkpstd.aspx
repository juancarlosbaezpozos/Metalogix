<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=12.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <SharePoint:ListItemProperty runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    <SharePoint:ListItemProperty runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageImage" runat="server">
    <SharePoint:AlphaImage ID=onetidtpweb1 Src="/_layouts/images/wiki.png" Width=145 Height=54 Alt="" Runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <META Name="CollaborationServer" Content="SharePoint Team Web Site">
    <script>
	var navBarHelpOverrideKey = "wssmain";
	</script>
    <SharePoint:RssLink runat="server"/>
    <style type="text/css">
		.ms-siteactionspacer {
			display: none;
		}
</style>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderSearchArea" runat="server">
    <SharePoint:DelegateControl runat="server"
                                ControlId="SmallSearchInputBox"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMiniConsole" runat="server">
    <SharePoint:FormComponent TemplateName="WikiMiniConsole" ControlMode="Display" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderLeftActions" runat="server">
    <SharePoint:RecentChangesMenu runat="server" id="RecentChanges"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <SharePoint:FormField FieldName="WikiField" ControlMode="Display" runat="server"/>
    <TABLE class="ms-formtable" border=0 cellpadding=0 id="formTbl" cellspacing=0 width=100%>
        <SharePoint:ListFieldIterator
            ControlMode="Display"
            TemplateName="WideFieldListIterator"
            ExcludeFields="FileLeafRef;#WikiField"
            runat="server"/>
    </TABLE>
    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="Bottom" Title="loc:Bottom"/>
    <table border=0 cellpadding=2 cellspacing=0 width=100%>
        <tr>
            <td class="ms-descriptiontext" ID=onetidinfoblock2>
                <SharePoint:FormattedString FormatText="<%$Resources:wss,form_modifiedby%>" runat="server">
                    <SharePoint:FormField ControlMode="Display" FieldName="Modified" runat="server"/>
                    <SharePoint:FormField ControlMode="Display" FieldName="Editor" runat="server"/>
                </SharePoint:FormattedString>
            </td>
        </tr>
    </table>
</asp:Content>