   M S B A M L   `   `   `  ����N  JSubscriptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:SubscriptionManager.HelpersSubscriptionManager.Helpers  5      ,    &SubscriptionManager.Views.SettingsView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:SubscriptionManager.Helpers     5      .��5	      � $��10q�6   .+�5
   
   �� .@�5      �� $��General Settings��6   .��5      �� .��5      �� $��20q�6!   $	��600��6-   $
��Left=�6<   .+�5      �� $��General Settings��6$   $��18(�6<   $
��Bold"�6J   $��0,0,0,20q�6\   �� 5      $��Company Information��6#   $��0,0,0,20q�6@   $��15q�6R   .��5      �� .+�5   "   �� $��Company Name:��6(   �� 5   "   $��0,0,0,15q�5   *   ��5   *   �� *Settings.CompanyName+  �� UpdateSourceTrigger$  PropertyChanged=�5   *   �� 5   "   $��Admin Email:��6(   �� 5   "   $��0,0,0,10q�5   *   ��5   *   �� *Settings.AdminEmail+$  PropertyChanged=�5   *   5      5      �� 5      $��Billing Settings��6#   $��0,0,0,20q�6=   $��15q�6O   .��5      �� .+�5   "   �� $!��Default Price Per Unit ($):��6(   �� 5   "   $��0,0,0,15q�5    *   ��5   *   �� *Settings.DefaultPricePerUnit+$  PropertyChanged=�,   &SubscriptionManager.Helpers.Converters   DecimalConverter �� 	Converter# Z 5    *   �� 5"   "   $"��Billing Day of Month (1-28):��6(   �� 5#   "   $��0,0,0,15q�5$   *   ��5#   *   �� *Settings.BillingDay+$  PropertyChanged=�5$   *   �� 5&   "   $!��Auto-calculate monthly fees��6+   $��0,0,0,10q�5(   +    d� 	IsChecked 5'   +   �� *#!Settings.AutoCalculateMonthlyFees+5(   +   5)      5*      �� 5,       �� Orientation$ 
Horizontal=�6%   $��Center=�6>   $��0,20,0,0q�6[   .+�5-      �� $��Save Settings��6%   
��� � �
�������$��20,10q�50   %   $��0,0,15,0q�65   ��5.   %   �� *SaveSettingsCommand+50   5   �� 51      $��Refresh��6%   $��20,10q�53   %   ��52   %   �� *RefreshCommand+53   %   54      55      56      57      �� 59      $��Subscription Types��6   .��5:      � $��20q�6    � RowDefinitions 5;      �� 5<      $
��Auto �6(   �� 5=      $
��Auto �6(   �� 5>      $��* �6(   .+�5A      �� $��0��6    $#��Subscription Types Management��6-   $��18(�5B       $
��Bold"�6.   $��0,0,0,20q�6@   �� 5D      $��1��6!   $ 
Horizontal=�6.   $��0,0,0,15q�6G   .+�5E      �� $��Add New Subscription Type��6!   
��� � �
�������$
��15,8q�5H   !   $��0,0,15,0q�60   ��5F   !   �� *&$OpenNewSubscriptionTypeDialogCommand+5H   0   �� 5I      $��Edit Selected��6!   $
��15,8q�5K   !   $��0,0,15,0q�60   ��5J   !   �� *EditSubscriptionTypeCommand+{�5L   !   �� *SelectedSubscriptionType+   IsNotNullConverter# Z �� 5M      $��Delete Selected��6!   
���  ��
�������$
��15,8q�5P   !   $��0,0,15,0q�60   ��5N   !   �� *DeleteSubscriptionTypeCommand+{�5Q   !   �� *SelectedSubscriptionType+# Z �� 5R      $��Refresh��6!   $
��15,8q�5T   !   ��5S   !   �� *RefreshCommand+5T   !   5U      &   System.Windows.Controls.DataGrid  5W      $��2��6      AutoGenerateColumns$ False��5Z         
IsReadOnly$
 True��5[      	   AlternatingRowBackground
	 �������5X      �� *SubscriptionTypes+
 �� SelectedItem
 5Y      �� *SelectedSubscriptionType+   Columns 5]      0  *System.Windows.Controls.DataGridTextColumn  5^      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��61      Width5  /System.Windows.Controls.DataGridLengthConverter$	 150 6X   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6?   �� *Name+6X     5_      $ Description��61   $	 200 6f    6F   �� *Description+6f     5`      $ Monthly Fee��61   $	 100 6v    6F   �� *
MonthlyFee+ �� StringFormat$ C2��6v     5a      $ Category��61   $	 100 6`    6C   �� *
Category+6`   4  .System.Windows.Controls.DataGridCheckBoxColumn  5b      $ Active��65   $ 60 6b    6E   �� *
IsActive+6b     5c      $ Created��61   $	 100 6p    6B   �� *	CreatedAt+$ d��6p   5e      5f      5g      5h      �� 5j   
   ���	#80000000y�5k      �� *!IsNewSubscriptionTypeDialogOpen+    BoolToVisibilityConverter# Z ._�5l      �� 
�������$	��550��5m      $	��550��6!   $��Center=�5n      $��Center=�60    �� CornerRadius$ 10��5o      
�������$��1q�5p   (   ._�5q      �� $
��Auto=�6   $��20q�6B   .��5r      �� .+�5s      �� $
��Bold"�6$   $��16(�66   $��Center=�5t   $   $��0,0,0,20q�6A   ��5u      �� 5v   "    �� Path$ IsEditingSubscriptionType�6*    5w   &   7   1SubscriptionManager.Helpers.BoolToStringConverter  5x   *      	TrueValue$ Edit Subscription Type��6F      
FalseValue$ Add New Subscription Type��6i   5z   #   5|      �� 5~      $��Name:��6    �� 5      $��0,0,0,15q�5�   "   ��5   "   �� *NewSubscriptionType.Name+$  PropertyChanged=�5�   "   �� 5�      $��Description:��6    �� 5�      $��80��5�   "    �� TextWrapping$
 Wrap=�6.    �� AcceptsReturn$
 True��6B   " �� VerticalScrollBarVisibility$
 Auto=�5�   "   $��0,0,0,15q�5�   "   ��5�   "   �� *!NewSubscriptionType.Description+$  PropertyChanged=�5�   "   �� 5�      $��Monthly Fee ($):��6    �� 5�      $��0,0,0,15q�5�   "   ��5�   "   �� * NewSubscriptionType.MonthlyFee+$  PropertyChanged=�# Z 5�   "   �� 5�      $��	Category:��6    �� 5�      $��0,0,0,15q�5�   #   ��5�   #   �� *SubscriptionCategories+
 5�   #   �� *NewSubscriptionType.Category+5�   #   �� 5�      $��Active��6#   $��0,0,0,25q�5�   #    5�   #   �� *NewSubscriptionType.IsActive+5�   #   �� 5�      $ 
Horizontal=�6%   $��Center=�6>   .+�5�      �� $��Cancel��6%   $��0,0,15,0q�5�   %   $��25,10q�67   ��5�   %   �� *$"CloseSubscriptionTypeDialogCommand+5�   7   �� 5�      $
��Save��6%   
��� � �
�������$��25,10q�5�   %   ��5�   %   �� *SaveSubscriptionTypeCommand+5�   %   5�      5�      5�      5�      5�      5�      5�      