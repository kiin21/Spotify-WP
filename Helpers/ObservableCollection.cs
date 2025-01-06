using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Represents an observable collection that also listens to property changes of its items.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public sealed class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    where T : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class.
    /// </summary>
    public ObservableCollection()
    {
        CollectionChanged += FullObservableCollectionCollectionChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="pItems">The collection from which the elements are copied.</param>
    public ObservableCollection(IEnumerable<T> pItems) : this()
    {
        foreach (var item in pItems)
        {
            this.Add(item);
        }
    }

    /// <summary>
    /// Handles the CollectionChanged event of the collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Object item in e.NewItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
            }
        }
        if (e.OldItems != null)
        {
            foreach (Object item in e.OldItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
            }
        }
    }

    /// <summary>
    /// Handles the PropertyChanged event of an item in the collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
        OnCollectionChanged(args);
    }
}
