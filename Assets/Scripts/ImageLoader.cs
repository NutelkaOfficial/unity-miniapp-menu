using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader Instance { get; private set; }

    [Header("Network")]
    public int maxConcurrentRequests = 4;
    public int timeoutSeconds = 10;
    public string urlTemplate = "http://data.ikppbb.com/test-task-unity-data/pics/{0}.jpg";

    [Header("Cache")]
    public int maxCachedItems = 30;

    class CacheEntry
    {
        public Texture2D tex;
        public Sprite sprite;
        public int lastAccessTick;
    }

    class Request
    {
        public int index;
        public int requestId;
        public UnityWebRequest uwr;
        public List<Action<int, Sprite, int>> callbacks = new List<Action<int, Sprite, int>>();
        public Coroutine coroutine;
    }

    Dictionary<int, CacheEntry> cache = new Dictionary<int, CacheEntry>();
    Queue<Request> queue = new Queue<Request>();
    Dictionary<int, Request> running = new Dictionary<int, Request>();
    int tickCounter = 0;
    int nextRequestId = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update() { tickCounter++; }

    public Sprite GetFromCache(int index)
    {
        if (cache.TryGetValue(index, out var e))
        {
            e.lastAccessTick = tickCounter;
            return e.sprite;
        }
        return null;
    }

    public int RequestImage(int index, Action<int, Sprite, int> onComplete, bool highPriority = false)
    {
        var sprite = GetFromCache(index);
        if (sprite != null)
        {
            onComplete?.Invoke(index, sprite, 0);
            return 0;
        }

        if (running.TryGetValue(index, out var rRun))
        {
            rRun.callbacks.Add(onComplete);
            return rRun.requestId;
        }

        foreach (var r in queue)
        {
            if (r.index == index)
            {
                r.callbacks.Add(onComplete);
                return r.requestId;
            }
        }

        var req = new Request { index = index, requestId = nextRequestId++ };
        req.callbacks.Add(onComplete);

        if (highPriority)
        {
            var list = new List<Request>(queue);
            queue.Clear();
            queue.Enqueue(req);
            foreach (var it in list) queue.Enqueue(it);
        }
        else queue.Enqueue(req);

        TryStartNext();
        return req.requestId;
    }

    public void CancelRequest(int index, int requestId)
    {
        var newQ = new Queue<Request>();
        while (queue.Count > 0)
        {
            var r = queue.Dequeue();
            if (r.index == index && (requestId == 0 || r.requestId == requestId))
            {
                continue;
            }
            newQ.Enqueue(r);
        }
        queue = newQ;

        if (running.TryGetValue(index, out var run))
        {
            if (requestId == 0 || run.requestId == requestId)
            {
                if (run.uwr != null)
                {
                    try
                    {
                        run.uwr.Abort();
                    }
                    catch {}
                }

                if (run.coroutine != null)
                    StopCoroutine(run.coroutine);

                running.Remove(index);
                TryStartNext();
            }
        }

    }

    void TryStartNext()
    {
        while (running.Count < maxConcurrentRequests && queue.Count > 0)
        {
            var r = queue.Dequeue();
            StartRunning(r);
        }
    }

    void StartRunning(Request r)
    {
        running[r.index] = r;
        r.coroutine = StartCoroutine(DownloadCoroutine(r));
    }

    IEnumerator DownloadCoroutine(Request r)
    {
        string url = string.Format(urlTemplate, r.index + 1);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            r.uwr = uwr;
            uwr.timeout = timeoutSeconds;
            var op = uwr.SendWebRequest();
            while (!op.isDone && !uwr.isNetworkError && !uwr.isHttpError) yield return null;
            bool success = !(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError);
            if (success)
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                AddToCache(r.index, tex, sp);
                var cbs = new List<Action<int, Sprite, int>>(r.callbacks);
                running.Remove(r.index);
                TryStartNext();
                foreach (var cb in cbs) cb?.Invoke(r.index, sp, r.requestId);
            }
            else
            {
                var cbs = new List<Action<int, Sprite, int>>(r.callbacks);
                running.Remove(r.index);
                TryStartNext();
                r.uwr = null;
                foreach (var cb in cbs) cb?.Invoke(r.index, null, r.requestId);
            }
        }
        yield break;
    }

    void AddToCache(int index, Texture2D tex, Sprite sp)
    {
        if (cache.ContainsKey(index)) return;
        var e = new CacheEntry { tex = tex, sprite = sp, lastAccessTick = tickCounter };
        cache[index] = e;
        if (cache.Count > maxCachedItems)
        {
            int oldestKey = -1;
            int oldestTick = int.MaxValue;
            foreach (var kv in cache)
            {
                if (kv.Value.lastAccessTick < oldestTick)
                {
                    oldestTick = kv.Value.lastAccessTick;
                    oldestKey = kv.Key;
                }
            }
            if (oldestKey != -1)
            {
                var rem = cache[oldestKey];
                if (rem.sprite != null) Destroy(rem.sprite);
                if (rem.tex != null) Destroy(rem.tex);
                cache.Remove(oldestKey);
            }
        }
    }
}
