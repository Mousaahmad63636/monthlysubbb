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
��Left=�5�      ��5�      �� *SaveReadingCommand+5�      5�      5�      �� 5�      $��Reading History��6   .��5�      � $��10q�6   {�6#   �� *SelectedCustomer+# Z  5�      �� 5�      $
��Auto �6(   �� 5�      $��* �6(   .+�5�      �� $��0��6!   $ 
Horizontal=�6.   $��0,0,0,10q�6G   .+�5�      �� $��Print Invoice��6!   
����  �
�������$
��15,8q�5�   !   $��0,0,10,0q�60   ��5�   !   �� *PrintInvoiceCommand+5�   0   �� 5�      $��Print Selected Reading��6!   
����  �
�������$
��15,8q�5�   !   $��0,0,10,0q�60   ��5�   !   �� *PrintSelectedReadingCommand+{�5�   !   �� *SelectedCounterHistory+# Z �� 5�      $��Export to PDF��6!   
��� d �
�������$
��15,8q�5�   !   $��0,0,10,0q�60   ��5�   !   �� *ExportInvoiceToPdfCommand+5�   0   �� 5�      $��Refresh History��6!   $
��15,8q�5�   !   ��5�   !   �� *RefreshHistoryCommand+5�   !   5�        5�      $��1��6   $ False��5�      $
 True��5�         AlternatingRowBackground
 �������5�      �� *CounterHistory+ 5�      �� *SelectedCounterHistory+ 5�        5�      $
 Date��61   $	 140 6n    6?   �� *
RecordDate+$ g��6n     5�      $ Previous��61   $ 80 6s    6C   �� *
OldCounter+$ N2��6s     5�      $	 New��61   $ 80 6n    6>   �� *
NewCounter+$ N2��6n     5�      $ Usage��61   $ 80 6o    6@   �� *	UnitsUsed+$ N2��6o     5�      $
 Rate��61   $ 80 6q    6?   �� *PricePerUnit+$ C3��6q     5�      $ Amount��61   $	 100 6q    6A   �� *
BillAmount+$ C2��6q   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $ Actions��65   $	 120 6F      CellTemplate 5�   "   �� 5�   &   .R�5�   *   �� $ 
Horizontal=�65   .+�5�   .   �� $��Print��65   $��2q�5�   5   $	��8,3q�6@   $��10(�6N   
����  �
���������5�   5   �� *'%DataContext.PrintSingleReadingCommand+  �� RelativeSource  �� ! �� AncestorType! DataGrid��5�   5   �� 5�   G   5�   +   5�   '   5�      5�      5�      5�      �� 5�      $��Payments��6   .��5�      � $��10q�6   {�6#   �� *SelectedCustomer+# Z  5�      �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $��* �6(   .+�5�      �� $��0��6   $��Current Bill��6,   $��0,0,0,15q�6B   $��15q�6T   .��5�      �   5�      �� 5�   "   $��* �63   �� 5�   "   $��* �63    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   .+�5�      �� $��0��6$   $��0��61   $��Usage Bill:��6A   �� 5�      $��0��6&   $��1��63   $
 True��5�   &   
���������5�   &   �� *SelectedCustomer.BillAmount+$ C2��5�   8   �� 5     $��1��6$   $��0��61   $��Monthly Fee:��6A   �� 5     $��1��6&   $��1��63   $
 True��5  &   
���������5  &   �� *)'SelectedCustomer.MonthlySubscriptionFee+$ C2��5  8   �� 5     $��2��6$   $��0��61   $��Total Bill:��6A   $
��Bold"�6W   �� 5     $��2��6&   $��1��63   $
 True��5	  &   
�������$
��Bold"�6Q   ��5  &   �� *#!SelectedCustomer.TotalMonthlyBill+$ C2��5	  Q   �� 5     $��3��6$   $��0��61   $��Amount to Pay ($):��6A   �� 5     $��3��6&   $��1��63   ��5  &   �� *PaymentAmount+$ PropertyChanged=�# Z �� 5     $��4��6$   $��0��61   $��Payment Method:��6A   �� 5     $��4��6'   $��1��64   ��5  '   �� *PaymentMethods+ 5  '   �� *SelectedPaymentMethod+5     5     �� 5     $��1��6!   $ 
Horizontal=�6.   $��0,0,0,15q�6G   .+�5     �� $��Settle Full Bill��6!   
��� � �
�������$
��15,8q�5  !   $��0,0,10,0q�60   ��5  !   �� *SettleFullBillCommand+5  0   �� 5     $��Record Payment��6!   $
��15,8q�5  !   $��0,0,10,0q�60   ��5  !   �� *RecordPaymentCommand+5  0   �� 5     $��Refresh Payments��6!   $
��15,8q�5!  !   ��5   !   �� *RefreshPaymentsCommand+5!  !   5"       5%     $��2��6   $ False��5(     $
 True��5)     
 �������5&     �� *PaymentHistory+ 5'     �� *SelectedPayment+ 5+       5,     $
 Date��61   $ 80 6o    6?   �� *PaymentDate+$ d��6o     5-     $ 	Receipt #��61   $	 100 6f    6D   �� *ReceiptNumber+6f     5.     $ 
Usage Bill��61   $ 80 6z    6E   �� *UsageBillAmount+$ C2��6z     5/     $ Monthly Fee��61   $ 80 6�    6F   �� *MonthlySubscriptionAmount+$ C2��6�     50     $ 
Total Bill��61   $ 80 6z    6E   �� *TotalBillAmount+$ C2��6z     51     $ Amount Paid��61   $ 80 6v    6F   �� *
AmountPaid+$ C2��6v     52     $ Balance��61   $ 80 6x    6B   �� *RemainingBalance+$ C2��6x     53     $ Method��61   $ 80 6c    6A   �� *PaymentMethod+6c     54     $ Status��61   $ 80 6A   "   ElementStyle" 55  "   �� 56  &   " ��6,   .�57  *   ��  � �� 6G   �� *Status+$#   PaymentStatusToColorConverter# Z# �� 58  *    �@��Padding$	 5,2q�6D   �� 59  *    �@��TextAlignment$ Center=�6J   5:  '    5<  "   �� 5=  &   $ �� Path$$ Status�6.   5?       5@     $ Notes��61   $ * 6Z    6@   �� *Notes+6Z   5B     5C     5D     5E     �� 5H  
   ���	#80000000$��2��5J     y�5I     �� *IsNewCustomerDialogOpen+ %   BoolToVisibilityConverter# Z% ._�5K     �� 
�������$	��450��5L     $	��500��6!   $��Center=�5M     $��Center=�60   & �� CornerRadius$& 10��5N     
�������$��1q�5O  (   ._�5P     �� $
��Auto=�6   $��20q�6B   .��5Q     �� .+�5R     �� $��Add New Customer��6$   $
��Bold"�6<   $��16(�6N   $��Center=�5S  $   $��0,0,0,20q�6A   �� 5U     $��Name:��6    �� 5V     $��0,0,0,10q�5W  "   ��5V  "   �� *NewCustomer.Name+$ PropertyChanged=�5W  "   �� 5Y     $��Phone Number:��6    �� 5Z     $��0,0,0,10q�5[  "   ��5Z  "   �� *NewCustomer.PhoneNumber+$ PropertyChanged=�5[  "   �� 5]     $��Price Per Unit ($):��6    �� 5^     $��0,0,0,10q�5_  "   ��5^  "   �� *NewCustomer.PricePerUnit+$ PropertyChanged=�# Z 5_  "   �� 5a     $��Subscription Type:��6    �� 5b     $ Id��5d  #   $
 Name��5e  #   $��0,0,0,10q�5f  #   ��5b  #   �� *AvailableSubscriptionTypes+ 5c  #   �� * NewCustomer.SubscriptionTypeId+5f  #   �� 5h     $��Initial Counter Reading:��6    �� 5i     $��0,0,0,20q�5j  "   ��5i  "   �� *NewCustomer.NewCounter+$ PropertyChanged=�# Z 5j  "   �� 5l     $ 
Horizontal=�6%   $��Center=�6>   .+�5m     �� $��Cancel��6%   $��0,0,10,0q�5o  %   $
��15,8q�67   ��5n  %   �� *CloseNewCustomerDialogCommand+5o  7   �� 5p     $��Save Customer��6%   
��� � �
�������$
��15,8q�5s  %   ��5q  %   �� *AddCustomerCommand+5s  %   5t     5u     5v     5w     5x     5y     5z     