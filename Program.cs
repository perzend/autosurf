using System;
using System.IO;
using System.Data;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Resources;
using System.ComponentModel;
using System.Collections.Generic;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Emulation;

namespace NewProject1
{
	/// <summary>
	/// Класс для запуска выполнения скрипта
	/// </summary>
	public class Program : IZennoCustomCode, IZennoCustomEndCode
	{
		/// <summary>
		/// GoodEnd
		/// </summary>
		/// <param name="instance">Объект инстанса выделеный для данного скрипта</param>
		/// <param name="project">Объект проекта выделеный для данного скрипта</param>
		public void GoodEnd(Instance instance, IZennoPosterProjectModel project)
		{
			// TODO insert your code of GoodEnd here
		}

		/// <summary>
		/// BadEnd
		/// </summary>
		/// <param name="instance">Объект инстанса выделеный для данного скрипта</param>
		/// <param name="project">Объект проекта выделеный для данного скрипта</param>
		public void BadEnd(Instance instance, IZennoPosterProjectModel project)
		{
			// TODO insert your code of BadEnd here
		}

		/// <summary>
		/// Метод для запуска выполнения скрипта
		/// </summary>
		/// <param name="instance">Объект инстанса выделеный для данного скрипта</param>
		/// <param name="project">Объект проекта выделеный для данного скрипта</param>
		/// <returns>Код выполнения скрипта</returns>
		public int ExecuteCode(Instance instance, IZennoPosterProjectModel project)
		{
			
			// Открыть страницу Яндекса и прогрузить ее
			Tab tab = instance.ActiveTab;
			if ((tab.IsVoid) || (tab.IsNull)) return -1;
			if (tab.IsBusy) tab.WaitDownloading();
			tab.Navigate("https://yandex.ru/");
			if (tab.IsBusy) tab.WaitDownloading();

			// Выбрать из файла случайную ключевую фразу для запроса
			Random r = new Random();
        	string[] string_array = File.ReadAllLines(@"C:\zapros.txt",Encoding.Default);
        	string random_string = string_array[r.Next(0,string_array.Length)];
						
			// Определить на странице строку поиска
    		HtmlElement input_search = tab.FindElementByXPath("//input[@class='input__control input__input']", 0);
    		if (input_search.IsVoid) return -2;
						
			// Определить на странице кнопку Найти
			HtmlElement find_button = tab.FindElementByXPath("//button[contains(@class,'button')]", 0);
			if (find_button.IsVoid) return -2;
			
			// Ввести ключевую фразу в строку поиска
			input_search.SetValue(random_string, instance.EmulationLevel, false);
			instance.WaitFieldEmulationDelay();
			
			// Кликнуть на кнопку Найти
			find_button.RiseEvent("click", instance.EmulationLevel);
			if (tab.IsBusy) tab.WaitDownloading();
						
			// Задать количество переходов по страницам с результами поиска
			for (int i=0; i<11; i++)
			{
				// Определить на странице результаты поиска и поместить в список
				HtmlElementCollection results_search = tab.FindElementsByXPath("//div[@class='organic__url-text']");
    			if (results_search.IsVoid) return -2;
				
				// Кликнуть на случайную ссылку поисковой выдачи
				results_search.Elements[r.Next(0,results_search.Count-1)].RiseEvent("click", instance.EmulationLevel);
			
				// Работать с табом, открывшимся по ссылке
				Tab tab2 = instance.ActiveTab;
				if (tab2.IsBusy) tab2.WaitDownloading();
				System.Threading.Thread.Sleep(2000);
				tab2.Close();
					
				// Определить на странице кнопку Дальше для перехода на след. страницу с результатами поиска
				HtmlElement next_button = tab.FindElementByXPath("//a[contains(@class,'pager__item_kind_next')]",0);
				if (next_button.IsVoid) return -2;
								
				// Кликнуть по кнопке Дальше для перехода на след. страницу с результатами поиска
				next_button.RiseEvent("click", instance.EmulationLevel);
			}
						
			return 0;
		}
	}
}