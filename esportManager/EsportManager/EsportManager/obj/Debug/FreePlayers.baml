   M S B A M L   `   `   `  ����H  DEsportManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EsportManagerEsportManager  5   	       EsportManager.FreePlayers   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:EsportManager     5   	     � Title$  Volní hráči��5   	   $	��450��6   $��409.325��6*    � 
ResizeMode$ NoResize=�6:   .��5	      � .+�5
   
   �� -   5        $��GamesComboBox��5
      $��Center=�6(   $��0,10,0,0q�6E   $	��Top=�6W   $	��300��6o   6{   &   System.Windows.Controls.DataGrid  5   
   -   5        $��FreePlayersGrid��5   !   $��12(�6   $��Center=�68   $	��300��6U   $��	0,50,0,10q�6a      
IsReadOnly .    AutoGenerateColumns .     CanUserAddRows .     CanUserReorderColumns .    CanUserResizeColumns .     CanUserResizeRows .     CanUserSortColumns .  	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Sekce��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5      �� *SectionName+  5      $
 Pozice��6%    5      �� *PositionName+  5      $

 Nick��6%    5      �� *Nick+  5      $
 Jméno��6%    5      �� *Name+  5      $
 Příjmení��6%    5      �� *	Surname+5      5      5      