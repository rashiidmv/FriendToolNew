   M S B A M L   `   `   `  ����G  CFriend.Infra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_?clr-namespace:Friend.Infra.CustomControls;assembly=Friend.InfraFriend.Infra.CustomControls  5      F BQueryWindow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:QueryWindow.ViewsQueryWindow.Views 5      C&clr-namespace:QueryWindow.UsercontrolsQueryWindow.Usercontrols 5      )clr-namespace:QueryWindowQueryWindow 5	         QueryWindow.Views.Main   5      RCustomControls?clr-namespace:Friend.Infra.CustomControls;assembly=Friend.Infra  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:QueryWindow.Views     5      =
mycontrols&clr-namespace:QueryWindow.Usercontrols     5      1QueryWindowclr-namespace:QueryWindow     5	        � 	Resources  5       �� Source�� 5   
   $" ../QueryWindowResources.xaml?�6   %    .��5      ]� ��. .+�5   
   9� $	��Top=�6   $
��Bold"�6O    9� ExpandDirection$
 Down=�5       9� 
IsExpanded .  ��5   (   �� *ServerConfigHeader+.��5      ]� ��. .+�5      �� $��Bottom=�6   $��Connect��61   |�.  �� 	IsDefault . $��Normal"�5       � ToolTip$ Connect to server��6u   $��Center=�6�   $
��Left=�6�   ��6   �� *GetDbNamesCommand+��6@   ��  �� ElementName$ txtPassword��6�   � 5       � Orientation$ 
Horizontal=�6   .+�5      ��  �� Orientation$ 
Horizontal=�6!   .+�5      �� $��Server name:��6$   $��Center=�68   $��Normal"�6T   �� 5      $��Normal"�6#   $ Enter server name��6x   ��67   �� *
ServerName+	 �� UpdateSourceTrigger$	 PropertyChanged=�.��6�   .��5   4   5      5      �� 5      -   5        
 ��Name$
 SqlAuthentication��5   �   $��Normal"�6   $��Italic$�64   $��Center=�6H   $��SQL Server Authentication��6c    d� 	IsChecked 6�   �� *IsSqlAuthentication+�� 5       $ 
Horizontal=�6!   .+�5!      �� $��Normal"�6%   $��
User name:��69   $��Center=�6K   �� 5"      $��Normal"�6#   $ Enter user name��5#      ��5"   7   �� *
UserName+{�6Q   �� *	IsChecked+$ SqlAuthentication��5#      5$      �� 5%      $ 
Horizontal=�6!   .+�5&      �� $��Normal"�6%   $��	Password:��69   $��Center=�6J   G� 5'      -   5         G�Name$ txtPassword��5'   ;   $��Normal"�6'   $ Enter password��5(      {�5'   P   �� *	IsChecked+$ SqlAuthentication��5(      5)      5*      5+      5,      �� 5-   
   $	��Top=�6   $ 
Horizontal=�6*   .+�5.      �� $��	Database:��6   $��Center=�6)   �� 5/      $	��Top=�6   $ Select database��6�   ��6,   �� *DataBaseNames+{�6R   �� *CanSelectDB+ �� SelectedValue 6t   �� *CurrentDatabase+6�   50      �� 51   
   $ 
Horizontal=�6   $��Bottom=�6.   .+�52      /   )Friend.Infra.CustomControls.MyBusyControl  $��16��6M   $��16��6X   $��Center=�6d   y�6+   �� *
BusyStatus+6d   �� 53      $��38��6    �� LineStackingStrategy$ BlockLineHeight=�56       �� 
LineHeight$ 18��58      $��WrapWithOverflow=�5:      ��54      �� *QueryResultStatus+��55      �� *QueryResultStatusColor+ 57      �� *QueryResultStatusToolTip+��59      �� *Width+ �� RelativeSource ��  �� AncestorType" � �� Mode$ FindAncestor=�5:      5<      ]� 5>   
   ��. .+�5?      � $	��Top=�6    � RowDefinitions 5@      �� 5A      �� 5B      .+�5D      +  %QueryWindow.Usercontrols.Intellisense  $��0��6*   � 5E      $��1��6   $
��Auto��6)   .+�5F      �� $��Hold last results��6   $��Center=�6`   $	��110��6{    6;   �� *HoldLastResult+6{   �� 5G      $��0q�6   $	��100��6(   $��Execute��64   |�.  . ��5H   )   �� *ExecuteQueryCommand+5I      5J      �� 5K      $��0,6,0,0q�6   $
��Auto=�6,   .��5L      ��    QueryResultTemplate#��[  5M      ��5N      �� *QueryAndResult+5O      5P      5Q      5R      