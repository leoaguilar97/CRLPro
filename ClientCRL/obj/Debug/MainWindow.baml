   M S B A M L   `   `   `  ����D  @ClientCRL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ClientCRL	ClientCRL  5   	   P LUi.SyntaxHighlightBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\Bclr-namespace:Ui.SyntaxHighlightBox;assembly=Ui.SyntaxHighlightBoxUi.SyntaxHighlightBox 5   	       ClientCRL.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:ClientCRL     5   	   Ucbox11Bclr-namespace:Ui.SyntaxHighlightBox;assembly=Ui.SyntaxHighlightBox     5   	     � Title$  Compi Report Language��5	   	   $	��651��6'   $
��1082��64    � 
ResizeMode$ NoResize=�6A    � Icon$ assets/icons/crl_logo.png��6W    � WindowStartupLocation$ CenterScreen=�6x   .��5
      �  � RowDefinitions 5   
   �� 5       � ColumnDefinitions 5   
   �� 5      .+�5   
   �� -   5         ��Name$ EditorContainer��5      $
��Left=�6.   $	��359��6I   $��	10,47,0,0q�6V   $	��Top=�6i   $	��627��6�    �� SelectedIndex$ 0��6�   o� 5   
   $
��Left=�6   $��32��6*   $��	10,10,0,0q�66   $	��Top=�6I   $
��1054��6a   .@�5      m� -   5         m�Name$ btnNewEditor��5      $��Nuevo��6-   $��32��6<   $��80��6H   	 m� Icon	 5      �� 5      $&�� assets/icons/appbar.page.add.png��6   $��
0,-5,-9,-6q�6F   5      m� 5      -   5        $ btnOpenFile��5      $��Abrir��6,   $��32��6;   $��80��6G   	 5      �� 5      $)��#assets/icons/appbar.folder.open.png��6   $��
0,-5,-9,-6q�6I   5      m� 5      -   5        $ btnSave��5      $��Guardar��6(   $��32��69   $��80��6E   	 5      �� 5      $"��assets/icons/appbar.save.png��6   $��
0,-5,-9,-6q�6B   5!      m� 5"      $��Guardar Como��6   $��32��6-   $	��115��69   	 5#      �� 5$      $"��assets/icons/appbar.disk.png��6   $��
0,-5,-9,-6q�6B   5&      m� 5'      -   5        $ btnCloseEditor��5'      $��Cerrar��6/   $��32��6?   $��80��6K   	 5(      �� 5)      $)��#assets/icons/appbar.page.delete.png��6   $��
0,-5,-9,-6q�6I   5+      �� 5,      $
��Left=�6   $��32��63   $��30��6?   6L   m� 5-      -   5        $ 
btnExecute��5-      $��Ejecutar��6+   $��32��6=   $��80��6I   	 5.      �� 5/      $*��$assets/icons/appbar.control.play.png��6   $��
0,-5,-9,-6q�6J   51      �� 52      $
��Left=�6   $��32��63   $��30��6?   m� 53      $��Ver errores��6   $��32��6,   $	��105��68   	 54      �� 55      $-��'assets/icons/appbar.clipboard.paper.png��6   $��
0,-5,-9,-6q�6M   57      m� 58      $��Visualizar resultados��6   $��32��66   $	��160��6B   	 59      �� 5:      $3��-assets/icons/appbar.clipboard.paper.check.png��6   $��
0,-5,-9,-6q�6S   5<      5=      �� 5>   
   $
��Left=�6   $	��359��60   $��
657,47,0,0q�6=   $	��Top=�6Q   $	��407��6i   .@�5?      �� $��TabItem��6   .��5@      � ���	#FFE5E5E5.+�5A      �� -   5        
 ��Name$
 scrlView��5A   #   .��5B      �� -   5         ��Name$ astImage��5B       5C      5D      5E      �� 5F      $��TabItem��6   .��5G      � ���	#FFE5E5E56   5H      5I      �� 5J   
   $
��Left=�6   $	��201��60   $��
10,411,0,0q�6=   $	��Top=�6Q   $
��1054��6i   .@�5K      �� $��TabItem��6   .��5L      � ���	#FFE5E5E5.+�5M      �� -	   5         ��Name$ console��5M   "   $
��Left=�63   $	��153��6N   $	��Top=�6[   $
��1028��6s   $��	10,10,0,0q�6�   . �5N      /� .T�5O      J� .*�6(   .*�5P   "   �� 5Q      5R      5S      5T      5U      �� 5V      $��TabItem��6   .��5W      � ���	#FFE5E5E56   5X      5Y      5[      5\      