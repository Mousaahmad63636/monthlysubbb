   M S B A M L   `   `   `  ����N  JSubscriptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:SubscriptionManager.HelpersSubscriptionManager.Helpers  5      +    %SubscriptionManager.Views.ExpenseView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:SubscriptionManager.Helpers     5      .��5	      � $��10q�6     � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Expense Management��6   $��18(�5      $
��Bold"�6"   $��0,0,0,15q�5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,15q�6;   .+�5      �� $��From:��6   $��Center=�6$   (  "System.Windows.Controls.DatePicker  5      $��5,0,15,0q�6<      SelectedDate 6   �� *	StartDate+6<   �� 5      $	��To:��6   $��Center=�6"     5      $��5,0,15,0q�6:    6   �� *	EndDate+6:   �� 5      $��Filter��6   $
��10,5q�5      $��0,0,15,0q�6$   ��5   &   �� *FilterCommand+5   $   �� 5      $��Add Expense��6   
��� � �
�������$
��10,5q�5    ;   $��0,0,15,0q�6J   ��5   +   �� *OpenNewExpenseDialogCommand+5    J   �� 5"      $��Refresh��6   $
��10,5q�5#      ��5"   '   �� *RefreshCommand+5#      5$      &   System.Windows.Controls.DataGrid  5&   
   $��2��6      AutoGenerateColumns$ False��5)         CanUserAddRows$ False��5*         AlternatingRowBackground
 �������5'      �� *
Expenses+ �� SelectedItem 5(      �� *SelectedExpense+   Columns 5,      0  *System.Windows.Controls.DataGridTextColumn  5-      ,  &System.Windows.Controls.DataGridColumn   Header$
 Date��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 100 6\   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 63   �� *Date+ �� StringFormat$ d��6\     5.      $ Reason��6%   $		 200 6P   
 65   �� *Reason+6P     5/      $ Category��6%   $		 120 6T   
 67   �� *
Category+6T     50      $ Amount��6%   $		 100 6a   
 65   �� *Amount+$ C2��6a     51      $ Notes��6%   $	 * 6N   
 64   �� *Notes+6N   4  .System.Windows.Controls.DataGridTemplateColumn  52      $ Actions��6)   $		 150 6:      CellTemplate 53      �� 54      .R�55      �� $ 
Horizontal=�6)   .+�56   "   �� $
��Edit��6)   $��2q�58   )   $	��5,2q�64   $��10(�6B   ��57   )   �� * DataContext.SaveExpenseCommand+ �� RelativeSource ��  �� AncestorType DataGrid58   B   �� 59   "   $��Delete��6)   
���  ��
�������$��2q�5<   )   $	��5,2q�64   $��10(�6B   ��5:   )   �� *" DataContext.DeleteExpenseCommand+ ��  DataGrid5<   B   5=      5>      5@      5B      �� 5D   
   $��3��6   
����ح� �� Padding$ 10q�65   $��0,10,0,0q�6B   ._�5E      �� $
��Bold"�5F      $��14(�6*   ��5E      �� *TotalExpenses+$ Total Expenses: {0:C2}��5F   *   5G      �� 5I   
   ���	#80000000$��4��5K      y�5J      �� *IsNewExpenseDialogOpen+,   &SubscriptionManager.Helpers.Converters    BoolToVisibilityConverter �� 	Converter# Z ._�5L      �� 
�������$	��500��5M      $	��550��6!   $��Center=�5N      $��Center=�60    �� CornerRadius$ 10��5O      
�������$��1q�5P   (   ._�5Q      �� $
��Auto=�6   $��20q�6B   .��5R      �� .+�5S      �� $��Add New Expense��6$   $
��Bold"�6;   $��16(�6M   $��Center=�5T   $   $��0,0,0,20q�6A   �� 5V      $��Date:��6      5W      $��0,0,0,15q�5X   %    5W   %   �� *NewExpense.Date+5X   %   �� 5Z      $��Reason:��6    �� 5[      $��0,0,0,15q�5\   "   ��5[   "   �� *NewExpense.Reason+ �� UpdateSourceTrigger$ PropertyChanged=�5\   "   �� 5^      $��	Category:��6    �� 5_      $��0,0,0,15q�5a   #   ��5_   #   �� *
Categories+ 5`   #   �� *NewExpense.Category+5a   #   �� 5c      $��Amount ($):��6    �� 5d      $��0,0,0,15q�5e   "   ��5d   "   �� *NewExpense.Amount+$ PropertyChanged=�   DecimalConverter# Z 5e   "   �� 5g      $��Notes:��6    �� 5h      $��80��5i   "    �� TextWrapping$
 Wrap=�6.    �� AcceptsReturn$
 True��6B   " �� VerticalScrollBarVisibility$
 Auto=�5j   "   $��0,0,0,25q�5k   "   ��5h   "   �� *NewExpense.Notes+$ PropertyChanged=�5k   "   �� 5m      $ 
Horizontal=�6%   $��Center=�6>   .+�5n      �� $��Cancel��6%   $��0,0,15,0q�5p   %   $��20,10q�67   ��5o   %   �� *CloseNewExpenseDialogCommand+5p   7   �� 5q      $��Save Expense��6%   
��� � �
�������$��20,10q�5t   %   ��5r   %   �� *AddExpenseCommand+5t   %   5u      5v      5w      5x      5y      5z      5{      