   M S B A M L   `   `   `  ����N  JSubscriptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:SubscriptionManager.HelpersSubscriptionManager.Helpers  5      +    %SubscriptionManager.Views.ExpenseView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:SubscriptionManager.Helpers     5      .��5	      � $��10q�6     � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Expense Management��6   $��18(�5      $
��Bold"�6"   $��0,0,0,15q�5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,15q�6;   .+�5      	� $��Monthly View��6   $��Center=�5      $��0,0,20,0q�65    d� 	IsChecked 5   1   �� *IsMonthlyView+��5      �� *SwitchToMonthlyViewCommand+5   5   	� 5      $��Date Range View��6   $��Center=�5      $��0,0,20,0q�65    5   4   �� *IsMonthlyView+,   &SubscriptionManager.Helpers.Converters   InverseBoolConverter �� 	Converter# Z ��5      �� *SwitchToDateRangeViewCommand+5   5   5      �� 5   
   $��2��6   $ 
Horizontal=�6"   $��0,0,0,15q�6;   .+�5       �� $ 
Horizontal=�6   y�62   �� *IsMonthlyView+    BoolToVisibilityConverter# Z .+�5!      �� $��Year:��6   $��Center=�6(   �� 5"      $��80��5$      $��5,0,15,0q�6&   ��5"      �� *AvailableYears+ �� SelectedItem 5#      �� *SelectedYear+5$   &   �� 5&      $��Month:��6   $��Center=�6)   �� 5'      $	��100��5)      $��5,0,15,0q�6'   ��5'      �� *AvailableMonths+ 5(      �� *SelectedMonth+��5*      �� 5+      .R�5,      �� ��6(   �� !   MonthNumberToNameConverter# Z 5-      5/      50      �� 52      $ 
Horizontal=�6   y�62   �� *IsMonthlyView+'    InverseBoolToVisibilityConverter# Z .+�53      �� $��From:��6   $��Center=�6(   (  "System.Windows.Controls.DatePicker  54      $��5,0,15,0q�6@   	   SelectedDate	 6   �� *	StartDate+6@   �� 56      $	��To:��6   $��Center=�6&     57      $��5,0,15,0q�6>   	 6   �� *	EndDate+6>   �� 59      $��Filter��6   $
��10,5q�5:      $��0,0,15,0q�6(   ��59   *   �� *FilterCommand+5:   (   5;      �� 5=      $��Add Expense��6   
��� � �
�������$
��10,5q�5>   ;   $��0,0,15,0q�6J   ��5=   +   �� *OpenNewExpenseDialogCommand+5>   J   �� 5@      $��Refresh��6   $
��10,5q�5A      ��5@   '   �� *RefreshCommand+5A      5B      &   System.Windows.Controls.DataGrid  5D   
   $��3��6   
   AutoGenerateColumns$
 False��5G         CanUserAddRows$ False��5H         AlternatingRowBackground
 �������5E      �� *
Expenses+ 5F      �� *SelectedExpense+   Columns 5J      0  *System.Windows.Controls.DataGridTextColumn  5K      ,  &System.Windows.Controls.DataGridColumn   Header$
 Date��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6\   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *Date+ �� StringFormat$ d��6\     5L      $ Reason��6%   $	 200 6P    65   �� *Reason+6P     5M      $ Category��6%   $	 120 6T    67   �� *
Category+6T     5N      $ Amount��6%   $	 100 6a    65   �� *Amount+$ C2��6a     5O      $ Notes��6%   $ * 6N    64   �� *Notes+6N   4  .System.Windows.Controls.DataGridTemplateColumn  5P      $ Actions��6)   $	 150 6:      CellTemplate 5Q      �� 5R      .R�5S      �� $ 
Horizontal=�6)   .+�5T   "   �� $
��Edit��6)   $��2q�5V   )   $	��5,2q�64   $��10(�6B   ��5U   )   �� * DataContext.SaveExpenseCommand+ �� RelativeSource ��  �� AncestorType DataGrid5V   B   �� 5W   "   $��Delete��6)   
���  ��
�������$��2q�5Z   )   $	��5,2q�64   $��10(�6B   ��5X   )   �� *" DataContext.DeleteExpenseCommand+ ��  DataGrid5Z   B   5[      5\      5^      5`      � 5b   
   $��4��6   $��0,10,0,0q�6    � ColumnDefinitions 5c      �� 5d      $��* �6#   �� 5e      $��* �6#   �� 5f      $��* �6#   �� 5g      $��* �6#   .+�5j      �� $��0��6   
����ح� �� Padding$ 10q�6<   $��0,0,5,0q�6I   ._�5k      �� .+�5l      �� $��Total Expenses��6    $
��Bold"�66   $��12(�6H   $��Center=�6V   �� 5m      $
��Bold"�5n       $��14(�62   $��Center=�6@   ��5m       �� *TotalExpenses+$ C2��5n   @   5o      5p      �� 5r      $��1��6   
������$ 10q�6=   $��2.5,0q�6J   y�5s      �� *IsMonthlyView+# Z ._�5t      �� .+�5u      �� $��Total Consumption��6    $
��Bold"�69   $��12(�6K   $��Center=�6Y   �� 5v      $
��Bold"�5w       $��14(�62   $��Center=�6@   ��5v       �� *TotalConsumption+$ 
{0:N2} kWh��5w   @   5x      5y      �� 5{      $��2��6   
�������$ 10q�6>   $��2.5,0q�6K   y�5|      �� *IsMonthlyView+# Z ._�5}      �� .+�5~      �� $��Total Revenue��6    $
��Bold"�65   $��12(�6G   $��Center=�6U   �� 5      $
��Bold"�5�       $��14(�62   $��Center=�6@   ��5       �� *TotalRevenue+$ C2��5�   @   5�      5�      �� 5�      $��3��6   $ 10q�6%   $��5,0,0,0q�62   ��5�      �� *TotalProfit+   ProfitToColorConverter# Z y�5�      �� *IsMonthlyView+# Z ._�5�      �� .+�5�      �� $��Total Profit��6    $
��Bold"�64   $��12(�6F   $��Center=�6T   �� 5�      $
��Bold"�5�       $��14(�62   $��Center=�6@   ��5�       �� *TotalProfit+$ C2��5�   @   5�      5�      5�      �� 5�   
   ���	#80000000$��5��5�      y�5�      �� *IsNewExpenseDialogOpen+# Z ._�5�      �� 
�������$	��500��5�      $	��550��6!   $��Center=�5�      $��Center=�60    �� CornerRadius$ 10��5�      
�������$��1q�5�   (   ._�5�      �� $
��Auto=�6   $��20q�6B   .��5�      �� .+�5�      �� $��Add New Expense��6$   $
��Bold"�6;   $��16(�6M   $��Center=�5�   $   $��0,0,0,20q�6A   �� 5�      $��Date:��6      5�      $��0,0,0,15q�5�   %   	 5�   %   �� *NewExpense.Date+5�   %   �� 5�      $��Reason:��6    �� 5�      $��0,0,0,15q�5�   "   ��5�   "   �� *NewExpense.Reason+ �� UpdateSourceTrigger$ PropertyChanged=�5�   "   �� 5�      $��	Category:��6    �� 5�      $��0,0,0,15q�5�   #   ��5�   #   �� *
Categories+ 5�   #   �� *NewExpense.Category+5�   #   �� 5�      $��Amount ($):��6    �� 5�      $��0,0,0,15q�5�   "   ��5�   "   �� *NewExpense.Amount+$ PropertyChanged=�   DecimalConverter# Z 5�   "   �� 5�      $��Notes:��6    �� 5�      $��80��5�   "    �� TextWrapping$
 Wrap=�6.    �� AcceptsReturn$
 True��6B   " �� VerticalScrollBarVisibility$
 Auto=�5�   "   $��0,0,0,25q�5�   "   ��5�   "   �� *NewExpense.Notes+$ PropertyChanged=�5�   "   �� 5�      $ 
Horizontal=�6%   $��Center=�6>   .+�5�      �� $��Cancel��6%   $��0,0,15,0q�5�   %   $��20,10q�67   ��5�   %   �� *CloseNewExpenseDialogCommand+5�   7   �� 5�      $��Save Expense��6%   
��� � �
�������$��20,10q�5�   %   ��5�   %   �� *AddExpenseCommand+5�   %   5�      5�      5�      5�      5�      5�      5�      