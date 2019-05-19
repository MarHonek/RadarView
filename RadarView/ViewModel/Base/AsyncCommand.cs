using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RadarView.ViewModel.Base
{
	/// <summary>
	/// Base async Command pro MVVM.
	/// </summary>
	public class AsyncCommand : ICommand
	{
		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;
		private bool _isExecuting;

		public AsyncCommand(Func<Task> execute) : this(execute, () => true)
		{
		}

		public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
		{
			this._execute = execute;
			this._canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return !(this._isExecuting && this._canExecute());
		}

		public event EventHandler CanExecuteChanged;

		public async void Execute(object parameter)
		{
			this._isExecuting = true;
			this.OnCanExecuteChanged();
			try {
				await this._execute();
			} finally {
				this._isExecuting = false;
				this.OnCanExecuteChanged();
			}
		}

		protected virtual void OnCanExecuteChanged()
		{
			this.CanExecuteChanged?.Invoke(this, new EventArgs());
		}
	}
}
