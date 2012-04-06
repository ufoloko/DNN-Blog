'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Business

 ''' <summary>
 ''' Blog module's own implementation of a category.
 ''' </summary>
 ''' <remarks></remarks>
 ''' <history>
 '''		[pdonker]	01/09/2010	updated class to support various interfaces
 '''		[pdonker]	01/09/2010	included new column for SEO purposes
 ''' </history>
 <Serializable(), XmlRoot("Tag")> _
 Public Class TagInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

#Region " Private Members "
  Private _TagId As Int32 = -1
  Private _Active As Boolean = True
  Private _Cnt As Int32 = 0
  Private _PortalId As Int32 = -1
  Private _Slug As String = "Default.aspx"
  Private _Tag As String = "New tag"
  Private _Weight As Decimal = 0
#End Region

#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(ByVal TagId As Int32, ByVal Active As Boolean, ByVal Cnt As Int32, ByVal PortalId As Int32, ByVal Slug As String, ByVal Tag As String, ByVal Weight As Decimal)
   Me.Active = Active
   Me.Cnt = Cnt
   Me.PortalId = PortalId
   Me.Slug = Slug
   Me.Tag = Tag
   Me.TagId = TagId
   Me.Weight = Weight
  End Sub
#End Region

#Region " Public Properties "

  Public Property TagId() As Int32
   Get
    Return _TagId
   End Get
   Set(ByVal Value As Int32)
    _TagId = Value
   End Set
  End Property

  Public Property Active() As Boolean
   Get
    Return _Active
   End Get
   Set(ByVal Value As Boolean)
    _Active = Value
   End Set
  End Property

  Public Property Cnt() As Int32
   Get
    Return _Cnt
   End Get
   Set(ByVal Value As Int32)
    _Cnt = Value
   End Set
  End Property

  Public Property PortalId() As Int32
   Get
    Return _PortalId
   End Get
   Set(ByVal Value As Int32)
    _PortalId = Value
   End Set
  End Property

  Public Property Slug() As String
   Get
    Return _Slug
   End Get
   Set(ByVal Value As String)
    _Slug = Value
   End Set
  End Property

  Public Property Tag() As String
   Get
    Return _Tag
   End Get
   Set(ByVal Value As String)
    _Tag = Value
   End Set
  End Property

  Public Property Weight() As Decimal
   Get
    Return _Weight
   End Get
   Set(ByVal Value As Decimal)
    _Weight = Value
   End Set
  End Property

#End Region

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[]	01/09/2010  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   Active = Convert.ToBoolean(Null.SetNull(dr.Item("Active"), Active))
   PortalId = Convert.ToInt32(Null.SetNull(dr.Item("PortalId"), PortalId))
   Slug = Convert.ToString(Null.SetNull(dr.Item("Slug"), Slug))
   Tag = Convert.ToString(Null.SetNull(dr.Item("Tag"), Tag))
   TagId = Convert.ToInt32(Null.SetNull(dr.Item("TagId"), TagId))
   Try
    Cnt = Convert.ToInt32(Null.SetNull(dr.Item("Cnt"), Cnt))
    Weight = Convert.ToDecimal(Null.SetNull(dr.Item("Weight"), Weight))
   Catch ex As Exception
   End Try

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	01/09/2010  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return TagId
   End Get
   Set(ByVal value As Integer)
    TagId = value
   End Set
  End Property
#End Region

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "active"
     Return PropertyAccess.Boolean2LocalizedYesNo(Me.Active, formatProvider)
    Case "cnt"
     Return (Me.Cnt.ToString(OutputFormat, formatProvider))
    Case "portalid"
     Return (Me.PortalId.ToString(OutputFormat, formatProvider))
    Case "slug"
     Return PropertyAccess.FormatString(Me.Slug, strFormat)
    Case "tag"
     Return PropertyAccess.FormatString(Me.Tag, strFormat)
    Case "tagid"
     Return (Me.TagId.ToString(OutputFormat, formatProvider))
    Case "weight"
     Return (Me.Weight.ToString(OutputFormat, formatProvider))
    Case Else
     PropertyNotFound = True
   End Select

   Return Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[]	01/09/2010  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(ByVal reader As XmlReader, ByVal ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[]	01/09/2010  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    Boolean.TryParse(readElement(reader, "Active"), Active)
    If Not Int32.TryParse(readElement(reader, "Cnt"), Cnt) Then
     Cnt = Null.NullInteger
    End If
    If Not Int32.TryParse(readElement(reader, "PortalId"), PortalId) Then
     PortalId = Null.NullInteger
    End If
    Slug = readElement(reader, "Slug")
    Tag = readElement(reader, "Tag")
    Decimal.TryParse(readElement(reader, "Weight"), Weight)
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Tag", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[]	01/09/2010  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Tag")
   writer.WriteElementString("TagId", TagId.ToString())
   writer.WriteElementString("Active", Active.ToString())
   writer.WriteElementString("Cnt", Cnt.ToString())
   writer.WriteElementString("PortalId", PortalId.ToString())
   writer.WriteElementString("Slug", Slug)
   writer.WriteElementString("Tag", Tag)
   writer.WriteElementString("Weight", Weight.ToString())
   writer.WriteEndElement()
  End Sub
#End Region

    End Class

End Namespace