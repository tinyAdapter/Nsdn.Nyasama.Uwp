using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Nsdn.Nyasama.Uwp.Utilities
{
    /// <summary>
    /// 提供 <see cref="ObservableCollection{T}"/> 扩展方法的类。
    /// 参考：https://social.msdn.microsoft.com/Forums/zh-CN/895d9dd8-8872-4d4c-b9d3-f0287fdf8442/uwp03gridviewlistview-?forum=window10app
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// 将整个 <see cref="ObservableCollection{T}"/> 添加到目标 <see cref="ObservableCollection{T}"/> 中。
        /// </summary>
        /// <typeparam name="T">集合中的元素类型。</typeparam>
        /// <param name="source"><see cref="ObservableCollection{T}"/> 源。</param>
        /// <param name="target">目标 <see cref="ObservableCollection{T}"/>。</param>
        public static void AddTo<T>(this ObservableCollection<T> source, ObservableCollection<T> target)
        {
            if (source == null)
            {
                return;
            }

            foreach (var item in source)
            {
                if (item != null && !target.Contains(item))
                {
                    target.Add(item);
                }
            }
        }

        /// <summary>
        /// 将指定集合的元素插入到 <see cref="ObservableCollection{T}"/> 的指定索引处。
        /// </summary>
        /// <typeparam name="T">集合中的元素类型。</typeparam>
        /// <param name="source"><see cref="ObservableCollection{T}"/> 源。</param>
        /// <param name="collection">应将其元素添加到 <see cref="ObservableCollection{T}"/> 的指定索引处的集合。</param>
        public static void InsertRange<T>(this ObservableCollection<T> source, int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                if (item != null && !source.Contains(item))
                {
                    source.Insert(index++, item);
                }
            }
        }

        /// <summary>
        /// 清除集合中的所有 <see cref="null"/> 对象。
        /// </summary>
        /// <typeparam name="T">集合中的元素类型。</typeparam>
        /// <param name="source"><see cref="ObservableCollection{T}"/> 源。</param>
        public static void ClearNull<T>(this ObservableCollection<T> source)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (item == null)
                {
                    source.RemoveAt(index);
                }
                index++;
            }
        }
    }


    /// <summary>
    /// 表示一个支持增量加载的动态数据集合，在添加项、移除项或刷新整个列表时，此集合将提供通知。
    /// </summary>
    /// <typeparam name="T">集合中的元素类型。</typeparam>
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        #region 内部类

        /// <summary>
        /// 表示加载更多项的方法。
        /// </summary>
        /// <param name="c">传播有关应取消操作的通知。</param>
        /// <param name="count">应加载更多项的数量。</param>
        /// <returns>返回一个已加载更多项的 <see cref="ObservableCollection{T}>"/>。</returns>
        public delegate Task<ObservableCollection<T>> LoadMoreItemsFuncAsync(CancellationToken c, uint count);

        #endregion

        #region 字段

        /// <summary>
        /// 表示加载更多项的方法。
        /// </summary>
        private LoadMoreItemsFuncAsync _loadMoreItemsFuncAsync;
        /// <summary>
        /// 支持增量加载实现的 Sentinel 值。
        /// </summary>
        private Func<bool> _hasMoreItems;

        #region 状态

        private bool _busy;
        private CancellationToken _cancellationToken;

        #endregion

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置该内部数据结构在不调整大小的情况下能够容纳的元素总数。
        /// </summary>
        public uint Capacity { get; set; }
        /// <summary>
        /// 获取 <see cref="IncrementalLoadingCollection{T}"/> 是否无元素数限制。
        /// </summary>
        public bool IsInfinite { get; private set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建一个无元素数限制的 <see cref="IncrementalLoadingCollection{T}"/> 实例，并使用指定参数初始化。
        /// </summary>
        /// <param name="loadMoreItemsFuncAsync">表示加载更多项的方法。</param>
        /// <param name="hasMoreItems">表示定义一组是否还有更多项可以加载的方法。</param>
        public IncrementalLoadingCollection(LoadMoreItemsFuncAsync loadMoreItemsFuncAsync, Func<bool> hasMoreItems)
            : this(loadMoreItemsFuncAsync, hasMoreItems, 0)
        { }

        /// <summary>
        /// 创建一个 <see cref="IncrementalLoadingCollection{T}"/> 实例，并使用指定参数初始化。
        /// </summary>
        /// <param name="loadMoreItemsFuncAsync">表示加载更多项的方法。</param>
        /// <param name="hasMoreItems">表示定义一组是否还有更多项可以加载的方法。</param>
        /// <param name="capacity">设置 <see cref="IncrementalLoadingCollection{T}"/> 能够容纳的元素总数。</param>
        public IncrementalLoadingCollection(LoadMoreItemsFuncAsync loadMoreItemsFuncAsync, Func<bool> hasMoreItems, uint capacity)
        {
            this._loadMoreItemsFuncAsync = loadMoreItemsFuncAsync;
            this._hasMoreItems = hasMoreItems;

            if (capacity == 0)
            {
                IsInfinite = true;
            }
            else
            {
                Capacity = capacity;
                IsInfinite = false;
            }
        }

        #endregion

        #region 实现 ISupportIncrementalLoading

        /// <summary>
        /// 获取支持增量加载实现的 Sentinel 值。
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (this.IsInfinite || this.Count <= Capacity)
                {
                    return this._hasMoreItems();
                }

                return false;
            }
        }

        /// <summary>
        /// 初始化从视图的增量加载。
        /// </summary>
        /// <param name="count">要加载的项的数目。</param>
        /// <returns>加载操作的换行结果。</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time.");
            }

            _busy = true;

            return AsyncInfo.Run((c) => InternalLoadMoreItemsAsync(c, count));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化从视图的增量加载的内部实现。
        /// </summary>
        /// <param name="c">传播有关应取消操作的通知。</param>
        /// <param name="count">要加载的项的数目。</param>
        /// <returns>加载操作的换行结果。</returns>
        private async Task<LoadMoreItemsResult> InternalLoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                _cancellationToken = c;

                var baseIndex = this.Count;
                uint numberOfItemsTogenerate = 0;

                if (!this.IsInfinite)
                {
                    if (baseIndex + count < this.Capacity)
                    {
                        numberOfItemsTogenerate = count;

                    }
                    else
                    {
                        // 如果有元素数限制，并且需要加载更多项的数量超出了该限制，则只加载集合空余数量的更多项。
                        numberOfItemsTogenerate = Capacity - (uint)(baseIndex);
                    }
                }
                else
                {
                    numberOfItemsTogenerate = count;
                }

                // 尝试执行加载更多项的开始事件。
                if (this.LoadMoreItemsStarted != null)
                {
                    this.LoadMoreItemsStarted(this, new LoadMoreItemsEventArgs(numberOfItemsTogenerate));
                }

                // 开始加载更多项。
                var intermediate = await this._loadMoreItemsFuncAsync(c, numberOfItemsTogenerate);

                // ATTN: #006 由于增量加载的机制，该段代码段将可能导致不按预期的结果，所以注释掉了( ▼-▼ )
                //if (intermediate.Count == 0)
                //{
                //    Capacity = (uint)this.Count;

                //    this.IsInfinite = false;
                //}
                //else
                //{
                // 将已加载的更多项添加到当前集合中。
                intermediate.AddTo(this);

                // 尝试执行加载更多项的完成事件。
                if (this.LoadMoreItemsCompleted != null)
                {
                    this.LoadMoreItemsCompleted(this, new LoadMoreItemsEventArgs((uint)intermediate.Count));
                }
                //}

                return new LoadMoreItemsResult { Count = (uint)intermediate.Count };
            }
            finally
            {
                this._busy = false;
            }
        }

        #endregion

        #region 委托与事件

        public class LoadMoreItemsEventArgs : EventArgs
        {
            public uint Count { get; }

            public LoadMoreItemsEventArgs(uint count)
            {
                this.Count = count;
            }
        }

        /// <summary>
        /// <see cref="LoadMoreItemsStarted"/> 的事件处理函数。
        /// </summary>
        /// <param name="count"></param>
        public delegate void LoadMoreItemsStartedEventHandler(object sender, LoadMoreItemsEventArgs e);
        /// <summary>
        /// <see cref="LoadMoreItemsCompleted"/> 的事件处理函数。
        /// </summary>
        /// <param name="count"></param>
        public delegate void LoadMoreItemsCompletedEventHandler(object sender, LoadMoreItemsEventArgs e);

        /// <summary>
        /// 当 <see cref="LoadMoreItemsAsync(uint)"/> 开始时发生。
        /// </summary>
        public event LoadMoreItemsStartedEventHandler LoadMoreItemsStarted;
        /// <summary>
        /// 当 <see cref="LoadMoreItemsAsync(uint)"/> 完成时发生。
        /// </summary>
        public event LoadMoreItemsCompletedEventHandler LoadMoreItemsCompleted;

        #endregion
    }
}