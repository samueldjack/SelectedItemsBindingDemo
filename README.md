SelectedItemsBindingDemo
========================

An example of how to bind the SelectedItems property of an ItemsControl in WPF to a ViewModel

The origin of all this and a good explanation of how it all works can be found at 
http://blog.functionalfun.net/2009/02/how-to-databind-to-selecteditems.html

The original example demo is still included in the code base, but the code has been reorganised a bit so that it can be published as a utility library in a NuGet package. I'm hoping you'll find it as useful as I have done, and making it available as a NuGet package should ease integration with your production code!

Example usage
-------------
```C#
List<string> names;
ObservableCollection<string> selectedNames;
ListView listView;

[TestInitialize]
public void TestInitialize()
{
  names = new List<string>() { "Abraham", "Lincoln", "James", "Buchanan" };
  selectedNames = new ObservableCollection<string>();
  listView = new ListView();
  listView.ItemsSource = names;
  listView.SelectionMode = SelectionMode.Extended;
  MultiSelectorBehaviours.SetSynchronizedSelectedItems(listView, (IList)selectedNames);
}

[TestMethod]
public void InitialiseToNoSelection()
{
  this.selectedNames.Count().Should().Be(0);
}

[TestMethod]
public void ShouldSynchroniseListViewSelectAll()
{
  // Act
  this.listView.SelectAll();

  // Assert
  this.selectedNames.Count().Should().Be(this.names.Count());
}

[TestMethod]
public void ShouldSynchroniseListViewSetSelectedIndex()
{
  // Act
  this.listView.SelectedIndex = 0;

  // Assert
  this.selectedNames.Count().Should().Be(1);
}

[TestMethod]
public void ShouldSynchroniseListViewSetSelectedItem()
{
  //Arrange
  Random random = new Random(DateTime.Now.Millisecond);
  object itemToSelect = this.listView.Items.GetItemAt(random.Next(this.names.Count));

  // Act
  this.listView.SelectedItem = itemToSelect;

  // Assert
  this.selectedNames.Count().Should().Be(1);
}

[TestMethod]
public void ShouldSynchroniseListViewAddSingleSelectedItem()
{
  //Arrange
  Random random = new Random(DateTime.Now.Millisecond);
  object itemToSelect = this.listView.Items.GetItemAt(random.Next(this.names.Count));

  // Act
  this.listView.SelectedItems.Add(itemToSelect);

  // Assert
  this.selectedNames.Count().Should().Be(1);
}

[TestMethod]
public void ShouldSynchroniseListViewAddMultipleSelectedItems()
{
  // Act
  this.listView.SelectedItems.Add(this.listView.Items.GetItemAt(0));
  this.listView.SelectedItems.Add(this.listView.Items.GetItemAt(1));

  // Assert
  this.selectedNames.ShouldBeEquivalentTo(this.listView.SelectedItems, opt => opt.WithStrictOrdering());
}