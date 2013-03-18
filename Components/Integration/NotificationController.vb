﻿'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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
Option Strict On
Option Explicit On

Imports DotNetNuke.Services.Social.Notifications
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Entities.Comments
Imports DotNetNuke.Modules.Blog.Integration.Integration
Imports DotNetNuke.Modules.Blog.Security.Security

Namespace Integration

 Public Class NotificationController

  ''' <summary>
  ''' This method will send a core notification to blog owners when a blog entry is pending publishing approval.
  ''' </summary>
  ''' <param name="objBlog"></param>
  ''' <param name="objEntry"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="title"></param>
  ''' <remarks></remarks>
  Public Shared Sub EntryPendingApproval(objBlog As BlogInfo, objEntry As EntryInfo, portalId As Integer, summary As String, title As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationPublishingTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName, objBlog.ModuleID, objEntry.BlogID, objEntry.ContentItemId, -1)
   Dim objNotification As New Notification

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = title
   objNotification.Body = summary
   objNotification.IncludeDismissAction = False
   objNotification.SenderUserID = objEntry.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, objBlog.OwnerUserId)
   Dim colUsers As List(Of UserInfo) = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, BlogPermissionTypes.APPROVE).Values.ToList
   If Not colUsers.Contains(objOwner) Then colUsers.Add(objOwner)

   NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)

  End Sub

  ''' <summary>
  ''' Removes any notifications associated w/ a specific blog entry pending approval.
  ''' </summary>
  ''' <param name="blogId"></param>
  ''' <param name="entryId"></param>
  ''' <remarks></remarks>
  Public Shared Sub RemoveEntryPendingNotification(moduleId As Integer, blogId As Integer, entryId As Integer)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationPublishingTypeName)
   Dim notificationKey As New NotificationKey(ContentTypeName, moduleId, blogId, entryId, -1)
   Dim objNotify As Notification = NotificationsController.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString).SingleOrDefault
   If objNotify IsNot Nothing Then
    NotificationsController.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID)
   End If
  End Sub

  ''' <summary>
  ''' This method will send a core notification to blog owners when a comment is pending approval.
  ''' </summary>
  ''' <param name="objComment"></param>
  ''' <param name="objBlog"></param>
  ''' <param name="objEntry"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="subject"></param>
  ''' <remarks></remarks>
  Public Shared Sub CommentPendingApproval(objComment As CommentInfo, objBlog As BlogInfo, objEntry As EntryInfo, portalId As Integer, summary As String, subject As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentApprovalTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentApprovalTypeName, objBlog.ModuleID, objBlog.BlogID, objEntry.ContentItemId, objComment.CommentID)
   Dim objNotification As New Notification

   Dim recipientId As Integer
   If objBlog.PublishAsOwner Then
    recipientId = objBlog.OwnerUserId
   Else
    recipientId = objEntry.CreatedByUserID
   End If

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = subject
   objNotification.Body = summary
   objNotification.IncludeDismissAction = True
   objNotification.SenderUserID = objComment.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, recipientId)
   Dim colUsers As List(Of UserInfo) = BlogPermissionsController.GetUsersByBlogPermission(portalId, objBlog.BlogID, BlogPermissionTypes.APPROVECOMMENT).Values.ToList
   If Not colUsers.Contains(objOwner) Then colUsers.Add(objOwner)

   NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)
  End Sub

  ''' <summary>
  ''' Removes any notifications associated w/ a specific blog comment pending approval.
  ''' </summary>
  ''' <param name="blogId"></param>
  ''' <param name="entryId"></param>
  ''' <remarks></remarks>
  Public Shared Sub RemoveCommentPendingNotification(moduleId As Integer, blogId As Integer, entryId As Integer, commentId As Integer)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentApprovalTypeName)
   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentApprovalTypeName, moduleId, blogId, entryId, commentId)
   Dim objNotify As Notification = NotificationsController.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationKey.ToString).SingleOrDefault
   If objNotify IsNot Nothing Then
    NotificationsController.Instance.DeleteAllNotificationRecipients(objNotify.NotificationID)
   End If
  End Sub

  ''' <summary>
  ''' This method will send a core notification to blog owners when a comment is added (they can only dismiss this notification)
  ''' </summary>
  ''' <param name="objComment"></param>
  ''' <param name="objEntry"></param>
  ''' <param name="objBlog"></param>
  ''' <param name="portalId"></param>
  ''' <param name="summary"></param>
  ''' <param name="subject"></param>
  ''' <remarks></remarks>
  Public Shared Sub CommentAdded(objComment As CommentInfo, objEntry As EntryInfo, objBlog As BlogInfo, portalId As Integer, summary As String, subject As String)
   Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(NotificationCommentAddedTypeName)

   Dim notificationKey As New NotificationKey(ContentTypeName & NotificationCommentAddedTypeName, objBlog.ModuleID, objBlog.BlogID, objEntry.ContentItemId, objComment.CommentID)
   Dim objNotification As New Notification

   Dim recipientId As Integer
   If objBlog.PublishAsOwner Then
    recipientId = objBlog.OwnerUserId
   Else
    recipientId = objEntry.CreatedByUserID
   End If

   objNotification.NotificationTypeID = notificationType.NotificationTypeId
   objNotification.Subject = subject
   objNotification.Body = summary
   objNotification.IncludeDismissAction = True
   objNotification.SenderUserID = objComment.CreatedByUserID
   objNotification.Context = notificationKey.ToString

   Dim objOwner As UserInfo = UserController.GetUserById(portalId, recipientId)
   Dim colUsers As New List(Of UserInfo)

   colUsers.Add(objOwner)

   NotificationsController.Instance.SendNotification(objNotification, portalId, Nothing, colUsers)
  End Sub

#Region " Install Methods "

  ''' <summary>
  ''' This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
  ''' </summary>
  ''' <remarks>This should only ever run once, during 5.0.0 install (via IUpgradeable)</remarks>
  Friend Shared Sub AddNotificationTypes()
   Dim actions As List(Of NotificationTypeAction) = New List(Of NotificationTypeAction)
   Dim deskModuleId As Integer = DesktopModuleController.GetDesktopModuleByFriendlyName("Blog").DesktopModuleID

   Dim objNotificationType As NotificationType = New NotificationType
   objNotificationType.Name = NotificationPublishingTypeName
   objNotificationType.Description = "Blog module and workflow approval."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    Dim objAction As New NotificationTypeAction
    objAction.NameResourceKey = "ApproveEntry"
    objAction.DescriptionResourceKey = "ApproveEntry_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/ApproveEntry"
    objAction.Order = 1
    actions.Add(objAction)

    objAction = New NotificationTypeAction
    objAction.NameResourceKey = "DeleteEntry"
    objAction.DescriptionResourceKey = "DeleteEntry_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/DeleteEntry"
    objAction.ConfirmResourceKey = "DeleteItem"
    objAction.Order = 3
    actions.Add(objAction)

    NotificationsController.Instance.CreateNotificationType(objNotificationType)
    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
   End If

   objNotificationType = New NotificationType
   objNotificationType.Name = NotificationCommentApprovalTypeName
   objNotificationType.Description = "Blog module and comment approval."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    actions.Clear()

    Dim objAction As New NotificationTypeAction
    objAction.NameResourceKey = "ApproveComment"
    objAction.DescriptionResourceKey = "ApproveComment_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/ApproveComment"
    objAction.Order = 1
    actions.Add(objAction)

    objAction = New NotificationTypeAction
    objAction.NameResourceKey = "DeleteComment"
    objAction.DescriptionResourceKey = "DeleteComment_Desc"
    objAction.APICall = "DesktopModules/Blog/API/NotificationService.ashx/DeleteComment"
    objAction.ConfirmResourceKey = "DeleteItem"
    objAction.Order = 3
    actions.Add(objAction)

    NotificationsController.Instance.CreateNotificationType(objNotificationType)
    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId)
   End If

   objNotificationType = New NotificationType
   objNotificationType.Name = NotificationCommentAddedTypeName
   objNotificationType.Description = "Blog module and comments being added."
   objNotificationType.DesktopModuleId = deskModuleId

   If NotificationsController.Instance.GetNotificationType(objNotificationType.Name) Is Nothing Then
    NotificationsController.Instance.CreateNotificationType(objNotificationType)
   End If
  End Sub

#End Region

 End Class

End Namespace