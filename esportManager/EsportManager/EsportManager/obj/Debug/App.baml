   M S B A M L   `   `   `  ����H  DEsportManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EsportManagerEsportManager  5          EsportManager.App   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -localclr-namespace:EsportManager     5        �� 	Resources  5         FocusVisual �� Property �� Value J� SnapsToDevicePixels �� StrokeDashArray  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground 
 MyButtonStyle �� 
TargetType �� 
TargetType ��Name	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName&   System.Windows.Controls.DataGrid �� Padding ��Name � ColumnDefinitions � RowDefinitions   SelectAllCommand �� 
ResourceId �� TypeInTargetAssembly7  1System.Windows.Controls.DataGridHeadersVisibility
   All �� ConverterParameter!   HeadersVisibilityConverter �� 	Converter �� RelativeSource �� AncestorTypeG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter  Name   Column:  4System.Windows.Controls.Primitives.DataGridRowHeader ��Name �� CanContentScroll ��Name  �� ScrollableHeight! � Maximum" �� Orientation*# �� #ComputedVerticalScrollBarVisibility$ �� ViewportHeight% �� ViewportSize& � Value' �� Mode( �� ScrollableWidth,) �� %ComputedHorizontalScrollBarVisibility* �� ViewportWidth+ �� Triggers, ]� 
Conditions- �� Property. �� Value��%�  &	        &	 �     &	 �     &	 �     &	     &	 6    &	 Y    &	 |    &	 �    &		 �    &	
 �    7   7  7  7  7  7  7  7  7  7 	 '  �    �� 5   
   .�5	      ��  � �� 5
      �� 5      .R�5      �� $��2q�6$    . #��� ��6J   $��1��6�   $	 1 2R�6�   5      5      5      �� 5   
   $ 	#FFDDDDDD��6;   �� 5   
   $ 	#FF707070��67   �� 5   
   $ 	#FFBEE6FD��6>   �� 5   
   $ 	#FF3C7FB1��6:   �� 5   
   $ 	#FFC4E5F6��6<   �� 5   
   $ 	#FF2C628B��68   �� 5   
   $ 	#FFF4F4F4��6=   �� 5   
   $ 	#FFADB2B5��69   �� 5   
   $ 	#FF838383��6=   �� 5   
   " ��6&   .�5      ��  �@�FocusVisualStyle8   61   �� 5       � ��8  6+   �� 5       � ��8  6,   �� 5       � ��# � ��6+   �� 5       � ��$ 1q�60   �� 5        � ��$ Center=�6;   �� 5!       � ��$ Center=�69   �� 5"       � ��$ 1q�6(   �� 5#       � �� 5$      �� 5%      " ��6&   .R�5&      �� $ border��6!   #��z��61   #��z��6]   #��z��6�    . ._�5'      �� $	 contentPresenter��6/   |�.  #��z��6[   #��z��6�   ��. # z 6�   #��z��6  5(      
 5)      P� 5*       �@��IsDefaulted . . �5+   "   �� $ border��6@   # � ��6T    � ��5,      P� 5-       �@J�IsMouseOver . . �5.   "   �� $ border��6?   8   � ��6S   �� 5/   "   $ border��6@   8   � ��6T   50      P� 51       � �� . . �52   "   �� $ border��6?   8   � ��6S   �� 53   "   $ border��6@   8   � ��6T   54      P� 55       � {� .  . �56   "   �� $ border��6?   8   � ��6S   �� 57   "   $ border��6@   8   � ��6T   �� 58   "   $ contentPresenter��6K   8 	  � ��6i   59      5;      5=      5>      �� 5@   
   "  6   .�5A      ��  � ��# � ��6+   �� 5B       � ��# � ��6+   �� 5C       � �� �	#FF688CAF6,   �� 5D       � ��$ 1q�60   �� 5E        �@ RowDetailsVisibilityMode$ VisibleWhenSelected=�69   �� 5F       � �� . 6>   �� 5G       �@��PanningMode$
 Both=�69   �� 5H       �@��IsFlicksEnabled .  67   �� 5J       � �� 5K      �� 5L      "  6&   .R�5N      �� 
�������$��1q�65   
�������# z��6`    . ._�5O      �� $ DG_ScrollViewer��6+   |�.  
���������5P   "   �� 5Q   &   " ��66   .R�5R   *   �  5S   .   �� 5T   2   $
��Auto �6C   �� 5U   2   $��* �6C   �� 5V   2   $
��Auto �6C    5X   .   �� 5Y   2   $
��Auto �6@   �� 5Z   2   $��* �6@   �� 5[   2   $
��Auto �6@   .+�5]   .   �� #��Z 65   |�.  ��6v   C� *�� $" DataGridSelectAllButtonStyle��"  +y�6�   �� *HeadersVisibility+# Z # Z  �� "  ��6�  �� *CellsPanelHorizontalOffset+ �� "    5^   .   $! PART_ColumnHeadersPresenter��6M   $��1��6r   ���	#FFD15050y�6�   �� *HeadersVisibility+# Z # Z  �� "  6e    5`   .   
����� �
���   �$��40(�6e   �� 5a   .   $! PART_ScrollContentPresenter��6E   # z��6j   $��2��6�   $��1��6�   �� 5b   .   $ PART_VerticalScrollBar��68   $��2��6X   #! z  6h   $" Vertical=�6�   $��1��6�   #y�z# 6�   #% z$ 6[  & 6�   �� *VerticalOffset+$' OneWay=� �� *TemplatedParent+6[  � 5c   .   $��1��63   $��2��6C    5d   2   �� 5e   6   ��6G   �� **(NonFrozenColumnsViewportHorizontalOffset+ �� "  �� 5f   6   $��* �6G   .+�5h   2   �� $ PART_HorizontalScrollBar��6<   $��1��6^   #! z( 6n   $" 
Horizontal=�6�   #y�z) 6�   #% z* 6Y  & 6�   �� *HorizontalOffset+$' OneWay=� �� *TemplatedParent+6Y  5i   /   5j   +   5k   '   .��5m   "   �� # z 61   5n      5o      5p      5r      + 5s      ]� 5t      , 5u      �� 5v      - �@��
IsGrouping. . 6:   �� 5w      "- �@%�IsVirtualizingWhenGrouping. .  6\   ./�5y      ��  � �� .  6F   5z      5|      5~      5      