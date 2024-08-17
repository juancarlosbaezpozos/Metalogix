<!-- _lcid="1033" _version="11.0.8161" _dal="1" -->
<!-- _LocalBinding -->
<%@ Page language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=11.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=11.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=11.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=11.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<html dir="ltr">
<HEAD>
    <META Name="GENERATOR" Content="Microsoft SharePoint">
    <META Name="ProgId" Content="SharePoint.WebPartPage.Document">
    <META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=utf-8">
    <META HTTP-EQUIV="Expires" content="0">

    <Title ID=onetidTitle>Web Part Page</Title>
    <Link REL="stylesheet" Type="text/css" HREF="/_layouts/<%= System.Threading.Thread.CurrentThread.CurrentUICulture.LCID %>/styles/ows.css">
    <!--mstheme--><SharePoint:Theme runat="server"/>
    <META Name="Microsoft Theme" Content="default">
    <META Name="Microsoft Border" Content="none">
    <link type="text/xml" rel='alternate' href="_vti_bin/spdisco.aspx"/>
</HEAD>
<body>
<form runat="server">
    <table cellpadding="0" cellspacing="0" border="0" width="100%" height="100%">
        <!-- Begin SharePoint Head Banner-->
        <tr>
            <td valign="top" width="100%">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <TR>
                        <TD COLSPAN=3 WIDTH=100%>
                            <!--Top bar-->
                            <table class="ms-bannerframe" border="0" cellspacing="0" cellpadding="0" width="100%">
                                <tr>
                                    <td nowrap valign="middle">
                                        <img ID=onetidHeadbnnr0 alt="Logo" src="/_layouts/images/logo.gif">
                                    </td>
                                    <td class=ms-banner width=99% nowrap ID="HBN100" valign="middle">
                                        <!--webbot bot="Navigation" 
                                        S-Type="sequence" 
                                        S-Orientation="horizontal" 
                                        S-Rendering="html" 
                                        S-Btn-Nml="<a ID='onettopnavbar#LABEL_ID#' href='#URL#' accesskey='J'>#LABEL#</a>"
                                        S-Btn-Sel="<a ID='onettopnavbar#LABEL_ID#' href='#URL#' accesskey='J'>#LABEL#</a>"
                                        S-Btn-Sep="&amp;nbsp;&amp;nbsp;&amp;nbsp;"
                                        B-Include-Home="FALSE" 
                                        B-Include-Up="FALSE" 
                                        S-Btn-Nobr="FALSE" 
                                        U-Page="sid:1002"
                                        S-Target startspan -->
                                        <SharePoint:Navigation LinkBarId="1002" runat="server"/>
                                        <!--webbot bot="Navigation" endspan -->
                                    </td>
                                    <td class=ms-banner>&nbsp;&nbsp;</td>
                                    <td nowrap class=ms-banner style="padding-right: 7px">
                                        <SharePoint:PortalConnection runat="server"/>
                                    </td>
                                </tr>
                            </table>
                        </TD>
                    </TR>
                </table>
            </td>
        </tr> <!-- End SharePoint Head Banner-->
        <!-- Begin WebPartPage Title -->
        <tr>
            <td valign="top" width="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:TitleBar" ID="TitleBar" LockLayout="true" AllowPersonalization="false"/>
            </td>
        </tr> <!-- End WebPartPage Title -->
        <tr>
            <td valign="top" width="100%" height="100%" style="margin:4px">
                <!-- Begin Panel Layout -->
                <PlaceHolder id="MSO_ContentDiv" runat="server">
                    <!--MSTableType="layout"-->
                    <table id="MSO_ContentTable" cellpadding="4" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" width="100%">
                                <WebPartPages:WebPartZone runat="server" Title="loc:FullPage" ID="FullPage"/>
                            </td>
                        </tr>
                        <script language="javascript">if(typeof(MSOLayout_MakeInvisibleIfEmpty) == "function") {MSOLayout_MakeInvisibleIfEmpty();}</script>
                    </table>
                </PlaceHolder> <!-- End Panel Layout -->
            </td>
        </tr>
    </table>
</form>
</body>
</html>