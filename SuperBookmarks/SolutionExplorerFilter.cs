using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Microsoft.Internal.VisualStudio.PlatformUI;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Konamiman.SuperBookmarks.Commands;

namespace Konamiman.SuperBookmarks
{
    [SolutionTreeFilterProvider(CommandBase.TheCommandsGuid, SolutionExplorerFilterCommand.TheCommandId)]
    public sealed class SolutionExplorerFilter : HierarchyTreeFilterProvider
    {
        SVsServiceProvider svcProvider;
        IVsHierarchyItemCollectionProvider hierarchyCollectionProvider;
        
        // Constructor required for MEF composition  
        [ImportingConstructor]
        public SolutionExplorerFilter(SVsServiceProvider serviceProvider, IVsHierarchyItemCollectionProvider hierarchyCollectionProvider)
        {
            this.svcProvider = serviceProvider;
            this.hierarchyCollectionProvider = hierarchyCollectionProvider;
        }

        // Returns an instance of Create filter class.  
        protected override HierarchyTreeFilter CreateFilter()
        {
            return new FilesWithBookmarksFilter(this.svcProvider, this.hierarchyCollectionProvider, SuperBookmarksPackage.Instance.BookmarksManager);
        }

        // Implementation of file filtering  
        private sealed class FilesWithBookmarksFilter : HierarchyTreeFilter
        {
            private readonly IServiceProvider svcProvider;
            private readonly IVsHierarchyItemCollectionProvider hierarchyCollectionProvider;
            private readonly BookmarksManager bookmarksManager;

            public FilesWithBookmarksFilter(
                IServiceProvider serviceProvider,
                IVsHierarchyItemCollectionProvider hierarchyCollectionProvider,
                BookmarksManager bookmarksManager)
            {
                this.svcProvider = serviceProvider;
                this.hierarchyCollectionProvider = hierarchyCollectionProvider;
                this.bookmarksManager = bookmarksManager;
            }

            // Gets the items to be included from this filter provider.   
            // rootItems is a collection that contains the root of your solution  
            // Returns a collection of items to be included as part of the filter  
            protected override async Task<IReadOnlyObservableSet> GetIncludedItemsAsync(IEnumerable<IVsHierarchyItem> rootItems)
            {
                IVsHierarchyItem root = HierarchyUtilities.FindCommonAncestor(rootItems);
                IReadOnlyObservableSet<IVsHierarchyItem> sourceItems;
                sourceItems = await hierarchyCollectionProvider.GetDescendantsAsync(
                                    root.HierarchyIdentity.NestedHierarchy,
                                    CancellationToken);

                IFilteredHierarchyItemSet includedItems = await hierarchyCollectionProvider.GetFilteredHierarchyItemsAsync(
                    sourceItems,
                    FilesWithBookmarksFilteredHierarchyItemSet.IsFileWithBookmarks,
                    CancellationToken);

                var wrapper = new FilesWithBookmarksFilteredHierarchyItemSet(sourceItems, includedItems);
                if(FilesWithBookmarksFilteredHierarchyItemSet.Instance != null)
                    FilesWithBookmarksFilteredHierarchyItemSet.Instance.Dispose();
                FilesWithBookmarksFilteredHierarchyItemSet.Instance = wrapper;

                return wrapper;
            }
        }

        public static void OnFileGotItsFirstBookmark(string path)
        {
            FilesWithBookmarksFilteredHierarchyItemSet.Instance?.OnItemAdded(path);
        }

        public static void OnFileLostItsLastBookmark(string path)
        {
            FilesWithBookmarksFilteredHierarchyItemSet.Instance?.OnItemRemoved(path);
        }

        //We need this to get realtime updates on the solution explorer
        //(files appear when they get their first bookmark, and disappear
        //when they lose their last bookmark) when the filter is on
        private class FilesWithBookmarksFilteredHierarchyItemSet : IFilteredHierarchyItemSet
        {
            private readonly IReadOnlyObservableSet<IVsHierarchyItem> allItems;
            private IFilteredHierarchyItemSet filteredItems;
            private List<IVsHierarchyItem> changedItem = new List<IVsHierarchyItem>(1);

            public static FilesWithBookmarksFilteredHierarchyItemSet Instance { get; set; }
            
            public FilesWithBookmarksFilteredHierarchyItemSet(IReadOnlyObservableSet<IVsHierarchyItem> allItems, IFilteredHierarchyItemSet filteredItems)
            {
                this.allItems = allItems;
                this.filteredItems = filteredItems;
                filteredItems.CollectionChanged += (sender, args) =>
                    CollectionChanged?.Invoke(sender, args);
                changedItem.Add(null); //must always have exactly 1 item
            }

            public void OnItemAdded(string path)
            {
                OnItemAddedOrRemoved(NotifyCollectionChangedAction.Add, path);
            }

            public void OnItemRemoved(string path)
            {
                OnItemAddedOrRemoved(NotifyCollectionChangedAction.Remove, path);
            }

            bool IsItemForFile(IVsHierarchyItem item, string filePath) =>
                HierarchyUtilities.IsPhysicalFile(item.HierarchyIdentity) &&
                item.CanonicalName.Equals(filePath, StringComparison.OrdinalIgnoreCase);
            
            private void OnItemAddedOrRemoved(NotifyCollectionChangedAction action, string path)
            {
                var hierarchyItem =
                    allItems.SingleOrDefault(i => IsItemForFile(i, path));
                if (hierarchyItem == null)
                    return;

                changedItem[0] = hierarchyItem;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }

            public int Count => filteredItems.Count;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public bool Contains(IVsHierarchyItem item) => filteredItems.Contains(item);

            public bool Contains(object item) => filteredItems.Contains(item);
            
            public void Dispose()
            {
                filteredItems.Dispose();
            }

            public IEnumerator<IVsHierarchyItem> GetEnumerator() => filteredItems.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => filteredItems.GetEnumerator();

            private static BookmarksManager BookmarksManager;

            static FilesWithBookmarksFilteredHierarchyItemSet()
            {
                BookmarksManager = SuperBookmarksPackage.Instance.BookmarksManager;
            }

            public static bool IsFileWithBookmarks(IVsHierarchyItem hierarchyItem)
            {
                if (hierarchyItem == null || !HierarchyUtilities.IsPhysicalFile(hierarchyItem.HierarchyIdentity))
                    return false;

                return BookmarksManager.HasBookmarks(Helpers.GetProperlyCasedPath(hierarchyItem.CanonicalName));
            }
        }
    }
}
