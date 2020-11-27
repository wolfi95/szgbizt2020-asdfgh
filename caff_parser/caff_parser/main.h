#ifdef __cplusplus
extern "C" {
#endif
#ifdef _WIN32
#  ifdef MODULE_API_EXPORTS
#    define MODULE_API __declspec(dllexport)
#  else
#    define MODULE_API __declspec(dllimport)
#  endif
#else
#  define MODULE_API
#endif
	//MODULE_API char* parseCaffToBmpStreamV1(const char* caffName);
	MODULE_API void parseCaffToBmpStreamV1(const char* caffName);
#ifdef __cplusplus
}
#endif