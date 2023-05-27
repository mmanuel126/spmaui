﻿using System.Threading.Tasks;

namespace spmaui.Controls
{
    public class SPSearchHandler : SearchHandler
    {
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                //ItemsSource = MonkeyData.Monkeys
                //    .Where(monkey => monkey.Name.ToLower().Contains(newValue.ToLower()))
                //    .ToList<Animal>();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            await Task.Delay(1000);

            // Note: strings will be URL encoded for navigation (e.g. "Blue Monkey" becomes "Blue%20Monkey"). Therefore, decode at the receiver.
            // This works because route names are unique in this application.
           // await Shell.Current.GotoAsync($"monkeydetails?name={((Animal)item).Name}");
            // The full route is shown below.
            // await Shell.Current.GotoAsync($"//animals/monkeys/monkeydetails?name={((Animal)item).Name}");
        }
    }
}
