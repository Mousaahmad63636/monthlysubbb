   M S B A M L   `   `   `  ����N  JSubscriptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:SubscriptionManager.HelpersSubscriptionManager.Helpers  5      0    *SubscriptionManager.Views.SubscriptionView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:SubscriptionManager.Helpers     5      .��5	      � $��10q�6     � ColumnDefinitions  5
   
   �� 5      $
��0.4* �6   �� 5      $
��0.6* �6   .+�5   
   � $��0��6   $��0,0,10,0q�6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� $��Monthly Subscriptions��6   $��18(�5      $
��Bold"�6&   $��0,0,0,10q�5      �� 5      $��1��6   $��0,0,0,10q�5      $��30��5      $��Center=�5      ��5      �� *
SearchText+ �� UpdateSourceTrigger$ PropertyChanged=�.��5   8   ��5       �� 5!       �� 
TargetType" ��6    �� Triggers 5"      P� 5#       P� Property � �� P� Value$  ��66   . �5$   "   ��  �� Property � �� �� Value 5%   &   !� 5&   *   	 j� 
AlignmentX$
	 Left=�66   
 j� 
AlignmentY$
 Center=�6H    j� Stretch$
 None=�6\    !� Visual 5'   .   �� 5(   2   $��Search customers...��68   
�������$��Italic$�5)   J   5+   +   5-   #   5.      50      .��51   !   52      &   System.Windows.Controls.DataGrid  54      $��2��6      AutoGenerateColumns$ False��57         
IsReadOnly$
 True��58         SelectionMode$ Single=�59      ��55      �� *	Customers+ �� SelectedItem 56      �� *SelectedCustomer+   Columns 5:      0  *System.Windows.Controls.DataGridTextColumn  5;      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6P   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *Name+6P     5<      $ Phone��6)   $	 100 6X    68   �� *PhoneNumber+6X     5=      $ Subscription��6)   $	 120 6h    6?   �� *SubscriptionTypeName+6h     5>      $ Current��6)   $ 80 6j    6:   �� *
NewCounter+ �� StringFormat$ N2��6j     5?      $ 
Usage Bill��6)   $ 80 6m    6=   �� *
BillAmount+$ C2��6m     5@      $ Monthly Fee��6)   $ 80 6z    6>   �� *MonthlySubscriptionFee+$ C2��6z     5A      $ 
Total Bill��6)   $ 80 6s    6=   �� *TotalMonthlyBill+$ C2��6s   5C      �� 5E      $��3��6    �� Orientation$ 
Horizontal=�6&   $��0,10,0,0q�6?   .+�5F      �� $��Add Customer��6   $��0,0,10,0q�5H      $
��10,5q�6+   ��5G      �� *OpenNewCustomerDialogCommand+5H   +   �� 5I      $��Refresh��6   $
��10,5q�5K      ��5J      �� *RefreshCommand+5K      5L      5M      �� 5O   
   $��1��6   .@�5P      �� $��Customer Details��6   .��5Q      �� .��5R      �� $��10q�6!   {�6-   �� *SelectedCustomer+,   &SubscriptionManager.Helpers.Converters   IsNotNullConverter �� 	Converter# Z .+�5S      �� $��Customer Information��6$   $
��Bold"�6@   $��14(�6R   $��0,0,0,10q�6`   �� 5U      $��Name:��6    �� 5V      $��0,0,0,10q�5W   "   ��5V   "   �� *SelectedCustomer.Name+$ PropertyChanged=�5W   "   �� 5Y      $��Phone Number:��6    �� 5Z      $��0,0,0,10q�5[   "   ��5Z   "   �� *SelectedCustomer.PhoneNumber+$ PropertyChanged=�5[   "   �� 5]      $��Price Per Unit ($):��6    �� 5^      $��0,0,0,10q�5_   "   ��5^   "   �� *SelectedCustomer.PricePerUnit+$ PropertyChanged=�   DecimalConverter# Z 5_   "   �� 5a      $��Subscription Type:��6    �� 5b       �� SelectedValuePath$ Id��5d   #    �� DisplayMemberPath$
 Name��5e   #   $��0,0,0,10q�5f   #   ��5b   #   �� *AvailableSubscriptionTypes+ �� SelectedValue 5c   #   �� *%#SelectedCustomer.SubscriptionTypeId+5f   #   �� 5h      $��Monthly Subscription Fee:��6    �� 5i       �� 
IsReadOnly$
 True��5j   "   
�������$��0,0,0,10q�5k   "   ��5i   "   �� *)'SelectedCustomer.MonthlySubscriptionFee+$ C2��5k   "   �� 5m      $��Current Counter Reading:��6    �� 5n      $
 True��5o   "   
�������$��0,0,0,10q�5p   "   ��5n   "   �� *SelectedCustomer.NewCounter+$ N2��5p   "   �� 5r      $��Usage Bill Amount:��6    �� 5s      $
 True��5t   "   
�������$��0,0,0,10q�5u   "   ��5s   "   �� *SelectedCustomer.BillAmount+$ C2��5u   "   �� 5w      $��Total Monthly Bill:��6    �� 5x      $
 True��5y   "   
�������$��0,0,0,20q�5z   "   ��5x   "   �� *#!SelectedCustomer.TotalMonthlyBill+$ C2��5z   "   �� 5|      $ 
Horizontal=�6%   .+�5}      �� $��Save Changes��6%   $��0,0,10,0q�5   %   $
��15,8q�67   ��5~   %   �� *SaveCustomerCommand+5   7   �� 5�      $��Delete Customer��6%   
���  ��
�������$
��15,8q�5�   %   ��5�   %   �� *DeleteCustomerCommand+5�   %   5�      5�      5�      5�      �� 5�      $��Meter Reading��6   .��5�      �� $��10q�6   {�6)   �� *SelectedCustomer+# Z .+�5�      �� $��Record New Reading��6    $
��Bold"�6:   $��14(�6L   $��0,0,0,10q�6Z   � 5�        5�      �� 5�      $��* �6/   �� 5�      $��* �6/   .+�5�      �� $��0��6%   $��0,0,10,0q�65   .+�5�      �� $��Previous Reading:��6$   �� 5�      $
 True��5�   &   
���������5�   &   �� *SelectedCustomer.NewCounter+$ N2��5�   8   5�      �� 5�      $��1��6%   .+�5�      �� $��New Reading:��6$   �� 5�      ��6&   �� *
NewReading+$ PropertyChanged=�# Z 5�      5�      �� 5�      $��Price Per Unit ($):��6   $��0,10,0,0q�6:   �� 5�      $	��200��5�      $
��Left=�6*   ��5�      �� *PricePerUnit+$ PropertyChanged=�# Z 5�   *   �� 5�      $��Save Reading��6   $��0,20,0,0q�5�      $
��15,8q�6/   $
��Left=�5�      ��5�      �� *SaveReadingCommand+5�      5�      5�      �� 5�      $��Reading History��6   .��5�        $ False��5�      $
 True��5�      ��5�      �� *CounterHistory+ 5�        5�      $
 Date��6-   $	 120 6j    6;   �� *
RecordDate+$ g��6j     5�      $ Previous��6-   $ 80 6o    6?   �� *
OldCounter+$ N2��6o     5�      $	 New��6-   $ 80 6j    6:   �� *
NewCounter+$ N2��6j     5�      $ Usage��6-   $ 80 6k    6<   �� *	UnitsUsed+$ N2��6k     5�      $
 Rate��6-   $ 80 6m    6;   �� *PricePerUnit+$ C2��6m     5�      $ Amount��6-   $ 80 6m    6=   �� *
BillAmount+$ C2��6m   5�      5�      5�      �� 5�   
   ���	#80000000$��2��5�      y�5�      �� *IsNewCustomerDialogOpen+    BoolToVisibilityConverter# Z ._�5�      �� 
�������$	��450��5�      $	��500��6!   $��Center=�5�      $��Center=�60    �� CornerRadius$ 10��5�      
�������$��1q�5�   (   ._�5�      �� $
��Auto=�6   $��20q�6B   .��5�      �� .+�5�      �� $��Add New Customer��6$   $
��Bold"�6<   $��16(�6N   $��Center=�5�   $   $��0,0,0,20q�6A   �� 5�      $��Name:��6    �� 5�      $��0,0,0,10q�5�   "   ��5�   "   �� *NewCustomer.Name+$ PropertyChanged=�5�   "   �� 5�      $��Phone Number:��6    �� 5�      $��0,0,0,10q�5�   "   ��5�   "   �� *NewCustomer.PhoneNumber+$ PropertyChanged=�5�   "   �� 5�      $��Price Per Unit ($):��6    �� 5�      $��0,0,0,10q�5�   "   ��5�   "   �� *NewCustomer.PricePerUnit+$ PropertyChanged=�# Z 5�   "   �� 5�      $��Subscription Type:��6    �� 5�      $ Id��5�   #   $
 Name��5�   #   $��0,0,0,10q�5�   #   ��5�   #   �� *AvailableSubscriptionTypes+ 5�   #   �� * NewCustomer.SubscriptionTypeId+5�   #   �� 5�      $��Initial Counter Reading:��6    �� 5�      $��0,0,0,20q�5�   "   ��5�   "   �� *NewCustomer.NewCounter+$ PropertyChanged=�# Z 5�   "   �� 5�      $ 
Horizontal=�6%   $��Center=�6>   .+�5�      �� $��Cancel��6%   $��0,0,10,0q�5�   %   $
��15,8q�67   ��5�   %   �� *CloseNewCustomerDialogCommand+5�   7   �� 5�      $��Save Customer��6%   
��� � �
�������$
��15,8q�5�   %   ��5�   %   �� *AddCustomerCommand+5�   %   5�      5�      5�      5�      5�      5�      5�      