var CACHE_STATIC_NAME = 'static-v7';

self.addEventListener('install',
    function(event) {
        console.log('[Service Worker] Installing Service Worker... ', event);
        event.waitUntil(
            caches.open(CACHE_STATIC_NAME)
            .then(function(cache) {
                console.log('[Service Worker] Precaching...');
                cache.addAll([              
                    '/Content/bootstrap.min.css',
                    '/Content/Site.css',
                    '/Content/calendar.png',
                    '/Content/delete.png',
                    '/Content/edit.png',
                    '/Content/enter.png',
                    '/Content/list.png'
                ]);
            })
        )
    });

self.addEventListener('activate',
    function (event) {
        console.log('[Service Worker] Activating Service Worker... ', event);
        event.waitUntil(
            caches.keys()
                .then(function(keyList) {
                    return Promise.all(keyList.map(function(key) {
                        if (key !== CACHE_STATIC_NAME) {
                            console.log('[Service Worker] Removing old cache.', key);
                            return caches.delete(key);
                        }
                    }));
                })
        );
        return self.clients.claim();
    });

self.addEventListener('fetch',
    function (event) {
        event.respondWith(
            fetch(event.request)
            .catch(function (err) {
                return caches.open('Offline')
                    .then(function (cache) {
                        return cache.match('https://www.pamtim.com/Offline/Offline');
                    });

            })
        );
    });

//self.addEventListener('fetch',
//    function (event) {

//        if (event.request.url.indexOf('/Pregled')) {
//            event.respondWith(
//                caches.open(CACHE_DYNAMIC_NAME)
//                .then(function(cache) {
//                    return fetch(event.request)
//                        .then(function(res) {
//                            cache.put(event.request, res.clone());
//                            return res;
//                        });
//                })
//            );
//        } else {
//            event.respondWith(
//                caches.match(event.request)
//                .then(function(response) {
//                    if (response) {
//                        return response;
//                    } else {
//                        return fetch(event.request)
//                            .then(function(res) {
//                                return caches.open(CACHE_DYNAMIC_NAME)
//                                    .then(function(cache) {
//                                        cache.put(event.request.url, res.clone());
//                                        return res;
//                                    })
//                            })
//                            .catch
//                            (function(err) {
//                                return caches.open('schedule&offline')
//                                    .then(function(cache) {
//                                        return cache.match('https://www.pamtim.com/Offline/Offline');
//                                    });
//                            });
//                    }
//                })
//            );
//        }     
//    });





//self.addEventListener('fetch',
//    function(event) {
//        event.respondWith(
//            caches.match(event.request)
//            .then(function(response) {
//                if (response) {
//                    return response;
//                } else {
//                    fetch(event.request)
//                        .catch(function(err) {
//                            return caches.open('Offline')
//                                .then(function(cache) {
//                                    return cache.match('https://www.pamtim.com/Offline/Offline');
//                                });
//                        })
//                }
//            })
//        );
//    });



//self.addEventListener('fetch',
//    function(event) {
//        event.respondWith(
//            fetch(event.request)
//            .catch(function(err) {
//                caches.match(event.request)
//                    .then(function(response) {
//                        if (response) {
//                            console.log('111');
//                            return response;
//                        } else {
//                            console.log('222');
//                            return caches.open('schedule&offline')
//                                .then(function(cache) {
//                                    return cache.match('https://www.pamtim.com/Offline/Offline');
//                                });
//                        }
//                    });
//            })
//        );
//    });


//self.addEventListener('fetch',
//    function (event) {
//        event.respondWith(
//            caches.match(event.request)
//                .then(function(response) {
//                if (response) {
//                    return response;
//                } else {
//                    return fetch(event.request)
//                        .then(function(res) {
//                            return caches.open(CACHE_DYNAMIC_NAME)
//                                .then(function(cache) {
//                                    cache.put(event.request.url, res.clone());
//                                    return res;
//                                })
//                        })
//                        .catch
//                        (function(err) {
//                            return caches.open('schedule&offline')
//                                .then(function(cache) {
//                                   return cache.match('https://www.pamtim.com/Offline/Offline');
//                                });
//                        });
//                }
//            })
//        );
//    });